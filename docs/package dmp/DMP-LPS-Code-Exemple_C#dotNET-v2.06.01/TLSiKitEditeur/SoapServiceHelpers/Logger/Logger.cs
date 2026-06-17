/**
* La présente mise à disposition du code exemple ne saurait être interprétée comme un quelconque transfert des droits de propriété sur celui-ci.
* L’utilisateur désigne ci-après l’entité destinataire du code exemple.
* L'attention de l’utilisateur est appelée sur les modalités d'utilisation du code exemple. 
* Ce dernier est fourni à titre d'information  permettant à l’utilisateur de réaliser librement l'adaptation personnalisée nécessaire à la création de l'interfaçage de son logiciel.  
* Le code exemple est transmis en son état de développement sans garantie, il n'a notamment pas fait l'objet de qualification sécuritaire.
* Le code exemple ne fait l'objet d'aucune maintenance.
* L’utilisateur est seul responsable des conditions de l’utilisation du code exemple et est libre de s'inspirer des éléments fournis et de les adapter par ses propres moyens à la situation particulière de la solution logicielle qu'il développe.
* Ainsi notamment, il est déconseillé de procéder par voie de copier-coller du code à partir des exemples fournis.
 */

using System;
using System.Diagnostics;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TLSiKitEditeur
{
    /// <summary>Loggeur générique</summary>
    public class Logger
    {
        private TraceSource ts = new TraceSource("KitEditeur");
        private LoggingLevelSwitch _levelSwitch;
        private static readonly Lazy<Logger> Lazy =
            new Lazy<Logger>(() => new Logger());

        private bool _isActive = true;

        public static Logger Log
        {
            get { return Lazy.Value; }
        }

        private Logger()
        {_levelSwitch = new LoggingLevelSwitch(initialMinimumLevel:LogEventLevel.Information);
            Serilog.Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(_levelSwitch).WriteTo.Console().WriteTo.RollingFile("log.log").CreateLogger();
        }



        public void Info(string message)
        {
            if (_isActive)
            {
                Serilog.Log.Information(message);
                ts.TraceEvent(TraceEventType.Information, 0, message);
            }       
        }

        public void Debug(string message)
        {
            if (_isActive)
            {
                Serilog.Log.Debug(message);
                ts.TraceEvent(TraceEventType.Verbose, 0, message);
            }
        } 
     

        public void Error(string message)
        {
            if (_isActive)
            {
                Serilog.Log.Error(message);
                ts.TraceEvent(TraceEventType.Error, 0, message);
            }    
        }

        public void EnableDebug()
        {
            _levelSwitch.MinimumLevel = LogEventLevel.Debug;
        }
        public void StartLogger()
        {
            _isActive = true;
        }

        public void StopLogger()
        {
            _isActive = false;
        }
    }
}