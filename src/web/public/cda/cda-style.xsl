<?xml version="1.0" encoding="UTF-8"?>
<!--
  Feuille de style CDA R2 — rendu HTML des volets CDA échangés via le DMP.
  Conforme au profil CI-SIS Volet de Structuration Minimale (ANS).
  Source: IHE PCC CDA Rendering Stylesheet, adapté pour l'ANS / CI-SIS.
  Versionnée dans le repo : src/web/public/cda/cda-style.xsl
  Chargée une seule fois par CdaService (cache en mémoire).
-->
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:n3="urn:hl7-org:v3"
  xmlns:in="urn:lantana-com:inline-variable-data"
  exclude-result-prefixes="n3 in">

  <xsl:output method="html" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

  <!-- ═══════════════════════════════════════════════════════════════
       Document root
  ═══════════════════════════════════════════════════════════════ -->
  <xsl:template match="/n3:ClinicalDocument">
    <div class="cda-document">
      <xsl:call-template name="cda-header"/>
      <xsl:call-template name="cda-body"/>
    </div>
  </xsl:template>

  <!-- ═══════════════════════════════════════════════════════════════
       En-tête clinique (titre, date, patient, auteur, établissement)
  ═══════════════════════════════════════════════════════════════ -->
  <xsl:template name="cda-header">
    <div class="cda-header">
      <h1 class="cda-doc-title">
        <xsl:value-of select="n3:title"/>
      </h1>
      <dl class="cda-meta">
        <xsl:if test="n3:effectiveTime/@value">
          <div class="cda-meta__row">
            <dt>Date du document</dt>
            <dd><xsl:call-template name="format-date"><xsl:with-param name="v" select="n3:effectiveTime/@value"/></xsl:call-template></dd>
          </div>
        </xsl:if>
        <!-- Patient -->
        <xsl:for-each select="n3:recordTarget/n3:patientRole">
          <xsl:variable name="pat" select="n3:patient"/>
          <xsl:if test="$pat/n3:name">
            <div class="cda-meta__row">
              <dt>Patient</dt>
              <dd>
                <xsl:call-template name="format-name">
                  <xsl:with-param name="name" select="$pat/n3:name[1]"/>
                </xsl:call-template>
              </dd>
            </div>
          </xsl:if>
          <xsl:if test="$pat/n3:birthTime/@value">
            <div class="cda-meta__row">
              <dt>Date de naissance</dt>
              <dd><xsl:call-template name="format-date"><xsl:with-param name="v" select="$pat/n3:birthTime/@value"/></xsl:call-template></dd>
            </div>
          </xsl:if>
          <xsl:if test="n3:id[@root='1.2.250.1.213.1.4.8' or @root='1.2.250.1.213.1.4.9']">
            <div class="cda-meta__row">
              <dt>INS</dt>
              <dd><xsl:value-of select="n3:id[@root='1.2.250.1.213.1.4.8' or @root='1.2.250.1.213.1.4.9']/@extension"/></dd>
            </div>
          </xsl:if>
        </xsl:for-each>
        <!-- Auteur -->
        <xsl:for-each select="n3:author[1]/n3:assignedAuthor">
          <xsl:variable name="aname" select="n3:assignedPerson/n3:name[1]"/>
          <xsl:if test="$aname">
            <div class="cda-meta__row">
              <dt>Auteur</dt>
              <dd>
                <xsl:call-template name="format-name">
                  <xsl:with-param name="name" select="$aname"/>
                </xsl:call-template>
              </dd>
            </div>
          </xsl:if>
          <xsl:if test="n3:representedOrganization/n3:name">
            <div class="cda-meta__row">
              <dt>Établissement</dt>
              <dd><xsl:value-of select="n3:representedOrganization/n3:name"/></dd>
            </div>
          </xsl:if>
        </xsl:for-each>
        <!-- Période de service -->
        <xsl:if test="n3:documentationOf/n3:serviceEvent/n3:effectiveTime">
          <xsl:variable name="low" select="n3:documentationOf/n3:serviceEvent/n3:effectiveTime/n3:low/@value"/>
          <xsl:variable name="high" select="n3:documentationOf/n3:serviceEvent/n3:effectiveTime/n3:high/@value"/>
          <xsl:if test="$low or $high">
            <div class="cda-meta__row">
              <dt>Période</dt>
              <dd>
                <xsl:if test="$low"><xsl:call-template name="format-date"><xsl:with-param name="v" select="$low"/></xsl:call-template></xsl:if>
                <xsl:if test="$low and $high"> — </xsl:if>
                <xsl:if test="$high"><xsl:call-template name="format-date"><xsl:with-param name="v" select="$high"/></xsl:call-template></xsl:if>
              </dd>
            </div>
          </xsl:if>
        </xsl:if>
      </dl>
    </div>
  </xsl:template>

  <!-- ═══════════════════════════════════════════════════════════════
       Corps (structuredBody → sections)
  ═══════════════════════════════════════════════════════════════ -->
  <xsl:template name="cda-body">
    <div class="cda-body">
      <xsl:apply-templates select="n3:component/n3:structuredBody/n3:component/n3:section"/>
    </div>
  </xsl:template>

  <xsl:template match="n3:section">
    <section class="cda-section">
      <xsl:if test="n3:title">
        <h2 class="cda-section__title"><xsl:value-of select="n3:title"/></h2>
      </xsl:if>
      <div class="cda-section__text">
        <xsl:apply-templates select="n3:text"/>
      </div>
      <!-- Sous-sections -->
      <xsl:apply-templates select="n3:component/n3:section"/>
    </section>
  </xsl:template>

  <!-- ═══════════════════════════════════════════════════════════════
       Éléments de texte narratif HL7 v3 (SD.TEXT)
  ═══════════════════════════════════════════════════════════════ -->
  <xsl:template match="n3:text">
    <xsl:apply-templates/>
  </xsl:template>

  <!-- Paragraphe -->
  <xsl:template match="n3:paragraph">
    <p class="cda-p"><xsl:apply-templates/></p>
  </xsl:template>

  <!-- Caption (titre de bloc) -->
  <xsl:template match="n3:caption">
    <span class="cda-caption"><xsl:apply-templates/></span>
  </xsl:template>

  <!-- Listes (ordered / unordered) -->
  <xsl:template match="n3:list[@listType='ordered']">
    <ol class="cda-list"><xsl:apply-templates select="n3:item"/></ol>
  </xsl:template>
  <xsl:template match="n3:list">
    <ul class="cda-list"><xsl:apply-templates select="n3:item"/></ul>
  </xsl:template>
  <xsl:template match="n3:item">
    <li><xsl:apply-templates/></li>
  </xsl:template>

  <!-- Tableau -->
  <xsl:template match="n3:table">
    <div class="cda-table-wrap">
      <table class="cda-table">
        <xsl:apply-templates/>
      </table>
    </div>
  </xsl:template>
  <xsl:template match="n3:thead">
    <thead><xsl:apply-templates/></thead>
  </xsl:template>
  <xsl:template match="n3:tbody">
    <tbody><xsl:apply-templates/></tbody>
  </xsl:template>
  <xsl:template match="n3:tfoot">
    <tfoot><xsl:apply-templates/></tfoot>
  </xsl:template>
  <xsl:template match="n3:tr">
    <tr><xsl:apply-templates/></tr>
  </xsl:template>
  <xsl:template match="n3:th">
    <th>
      <xsl:if test="@colspan"><xsl:attribute name="colspan"><xsl:value-of select="@colspan"/></xsl:attribute></xsl:if>
      <xsl:if test="@rowspan"><xsl:attribute name="rowspan"><xsl:value-of select="@rowspan"/></xsl:attribute></xsl:if>
      <xsl:apply-templates/>
    </th>
  </xsl:template>
  <xsl:template match="n3:td">
    <td>
      <xsl:if test="@colspan"><xsl:attribute name="colspan"><xsl:value-of select="@colspan"/></xsl:attribute></xsl:if>
      <xsl:if test="@rowspan"><xsl:attribute name="rowspan"><xsl:value-of select="@rowspan"/></xsl:attribute></xsl:if>
      <xsl:apply-templates/>
    </td>
  </xsl:template>

  <!-- Mise en forme inline -->
  <xsl:template match="n3:content">
    <span>
      <xsl:if test="@styleCode">
        <xsl:attribute name="class">
          <xsl:call-template name="style-code">
            <xsl:with-param name="code" select="@styleCode"/>
          </xsl:call-template>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates/>
    </span>
  </xsl:template>

  <xsl:template match="n3:br">
    <br/>
  </xsl:template>

  <!-- Lien de référence interne (footnote, etc.) -->
  <xsl:template match="n3:linkHtml">
    <a>
      <xsl:if test="@href"><xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute></xsl:if>
      <xsl:apply-templates/>
    </a>
  </xsl:template>

  <!-- renderMultiMedia (images encodées en base64 dans le CDA) -->
  <xsl:template match="n3:renderMultiMedia">
    <xsl:variable name="ref" select="@referencedObject"/>
    <xsl:variable name="obs" select="//n3:observationMedia[@ID=$ref]"/>
    <xsl:if test="$obs">
      <img alt="Image médicale intégrée">
        <xsl:attribute name="src">
          <xsl:text>data:</xsl:text>
          <xsl:value-of select="$obs/n3:value/@mediaType"/>
          <xsl:text>;base64,</xsl:text>
          <xsl:value-of select="normalize-space($obs/n3:value)"/>
        </xsl:attribute>
      </img>
    </xsl:if>
  </xsl:template>

  <!-- Supprimer les éléments non-textuels du corps clinique (entrées codées, etc.) -->
  <xsl:template match="n3:entry"/>

  <!-- ═══════════════════════════════════════════════════════════════
       Utilitaires
  ═══════════════════════════════════════════════════════════════ -->

  <!-- Formatage de date HL7 (YYYYMMDD[HHmmss[+TZ]]) → DD/MM/YYYY -->
  <xsl:template name="format-date">
    <xsl:param name="v"/>
    <xsl:variable name="len" select="string-length($v)"/>
    <xsl:choose>
      <xsl:when test="$len >= 8">
        <xsl:value-of select="substring($v,7,2)"/>/<xsl:value-of select="substring($v,5,2)"/>/<xsl:value-of select="substring($v,1,4)"/>
        <xsl:if test="$len >= 12">
          <xsl:text> </xsl:text><xsl:value-of select="substring($v,9,2)"/>:<xsl:value-of select="substring($v,11,2)"/>
        </xsl:if>
      </xsl:when>
      <xsl:otherwise><xsl:value-of select="$v"/></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Formatage d'un nom HL7 (family/given ou value) -->
  <xsl:template name="format-name">
    <xsl:param name="name"/>
    <xsl:choose>
      <xsl:when test="$name/n3:given or $name/n3:family">
        <xsl:value-of select="$name/n3:given"/>
        <xsl:if test="$name/n3:given and $name/n3:family"><xsl:text> </xsl:text></xsl:if>
        <xsl:value-of select="$name/n3:family"/>
      </xsl:when>
      <xsl:otherwise><xsl:value-of select="$name"/></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Conversion styleCode CDA → classe CSS -->
  <xsl:template name="style-code">
    <xsl:param name="code"/>
    <xsl:choose>
      <xsl:when test="$code='Bold'">cda-bold</xsl:when>
      <xsl:when test="$code='Italics'">cda-italic</xsl:when>
      <xsl:when test="$code='Underline'">cda-underline</xsl:when>
      <xsl:when test="$code='Emphasis'">cda-emphasis</xsl:when>
      <xsl:otherwise>cda-inline</xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
