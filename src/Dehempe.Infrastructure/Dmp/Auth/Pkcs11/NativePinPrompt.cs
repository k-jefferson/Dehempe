using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Auth.Pkcs11;

/// <summary>
/// Affiche un dialog natif (OS) pour saisir le code PIN de la carte CPS, à masquage activé.
/// Utilisé quand l'API tourne en local sur le poste du praticien et que le PIN n'a pas été
/// fourni via le header <c>X-Cps-Pin</c> (typiquement test via Swagger).
///
/// macOS : <c>osascript</c> (display dialog … with hidden answer).
/// Windows : <c>powershell</c> + WinForms (TextBox.UseSystemPasswordChar).
/// Autres plateformes : non supporté → retourne <c>null</c> (on retombe sur le flux 401).
/// </summary>
internal static class NativePinPrompt
{
    /// <summary>Délai max d'attente de la saisie utilisateur avant abandon.</summary>
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Demande le PIN via un dialog natif. Retourne le PIN saisi, ou <c>null</c> si l'utilisateur
    /// annule, si la plateforme n'est pas supportée, ou en cas d'erreur.
    /// </summary>
    public static string? TryPrompt(ILogger logger)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return RunMacOs(logger);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return RunWindows(logger);

            logger.LogWarning("Dialog PIN natif non supporté sur cette plateforme — flux header X-Cps-Pin requis.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Échec du dialog PIN natif : {Msg}", ex.Message);
            return null;
        }
    }

    private static string? RunMacOs(ILogger logger)
    {
        // Le dialog est affiché PAR « System Events » et celui-ci est `activate` → fenêtre au
        // PREMIER PLAN (sinon, lancé depuis un process Kestrel sans app GUI, le dialog passe
        // derrière). `with hidden answer` masque la saisie ; annulation → osascript sort ≠ 0.
        // (Peut déclencher un consentement TCC « osascript veut contrôler System Events » au
        // tout premier appel — à approuver une fois.)
        const string script =
            "tell application \"System Events\"\n" +
            "\tactivate\n" +
            "\tset r to display dialog \"Saisissez le code porteur de votre carte CPS\" " +
            "with title \"Déhempé — Carte CPS\" default answer \"\" with hidden answer " +
            "buttons {\"Annuler\", \"OK\"} default button \"OK\" cancel button \"Annuler\"\n" +
            "\treturn text returned of r\n" +
            "end tell";

        var psi = new ProcessStartInfo("/usr/bin/osascript")
        {
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
        };
        psi.ArgumentList.Add("-e");
        psi.ArgumentList.Add(script);

        return RunPrompt(psi, logger);
    }

    private static string? RunWindows(ILogger logger)
    {
        // WinForms masqué via PowerShell. Sortie : le PIN sur stdout, ou rien si annulé.
        const string script = @"
Add-Type -AssemblyName System.Windows.Forms, System.Drawing
$f = New-Object System.Windows.Forms.Form
$f.Text = 'Déhempé — Carte CPS'; $f.Width = 340; $f.Height = 170; $f.TopMost = $true
$f.StartPosition = 'CenterScreen'; $f.FormBorderStyle = 'FixedDialog'
$l = New-Object System.Windows.Forms.Label
$l.Text = 'Saisissez le code porteur de votre carte CPS :'; $l.AutoSize = $true; $l.Top = 15; $l.Left = 15
$t = New-Object System.Windows.Forms.TextBox
$t.UseSystemPasswordChar = $true; $t.Top = 45; $t.Left = 15; $t.Width = 290
$ok = New-Object System.Windows.Forms.Button
$ok.Text = 'OK'; $ok.Top = 85; $ok.Left = 150; $ok.DialogResult = [System.Windows.Forms.DialogResult]::OK
$ca = New-Object System.Windows.Forms.Button
$ca.Text = 'Annuler'; $ca.Top = 85; $ca.Left = 230; $ca.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
$f.Controls.AddRange(@($l, $t, $ok, $ca)); $f.AcceptButton = $ok; $f.CancelButton = $ca
if ($f.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) { [Console]::Out.Write($t.Text) }";

        var psi = new ProcessStartInfo("powershell")
        {
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
        };
        psi.ArgumentList.Add("-NoProfile");
        psi.ArgumentList.Add("-STA");
        psi.ArgumentList.Add("-Command");
        psi.ArgumentList.Add(script);

        return RunPrompt(psi, logger);
    }

    private static string? RunPrompt(ProcessStartInfo psi, ILogger logger)
    {
        using var proc = Process.Start(psi);
        if (proc is null) return null;

        if (!proc.WaitForExit((int)Timeout.TotalMilliseconds))
        {
            try { proc.Kill(entireProcessTree: true); } catch { /* best effort */ }
            logger.LogWarning("Dialog PIN natif : délai dépassé, saisie abandonnée.");
            return null;
        }

        if (proc.ExitCode != 0)
        {
            logger.LogInformation("Dialog PIN natif annulé par l'utilisateur.");
            return null;
        }

        var pin = proc.StandardOutput.ReadToEnd().Trim();
        return string.IsNullOrWhiteSpace(pin) ? null : pin;
    }
}
