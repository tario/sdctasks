<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output encoding="utf-8" method="html"
              omit-xml-declaration="yes" doctype-public="-//IETF//DTD HTML//EN"/>
  <xsl:variable name="outputPath">html/</xsl:variable>

  <xsl:param name="configFile"/>
  <xsl:variable name="docSet" select="string($configFile)"/>

  <!--Create document structure-->
<xsl:template match="/">
  <HTML>
    <HEAD>
      <meta name="GENERATOR" content="Paterns &amp; Practices Help Compiler" />
    </HEAD>
    <BODY>
      <UL>
        <xsl:apply-templates select="masterToc/toc"/>
      </UL>
    </BODY>
  </HTML>
</xsl:template>

<xsl:template match="topic">
  <LI>
    <OBJECT type="text/sitemap">
      <param name="Name">
        <xsl:attribute name="value"><xsl:value-of select="@title"/></xsl:attribute>
      </param>
      <param name="Local">
        <xsl:attribute name="value">
          <xsl:value-of select="$outputPath"/>
          <xsl:value-of select="@id"/>
          <xsl:text>.html</xsl:text>
        </xsl:attribute>
      </param>
    </OBJECT>
    <xsl:if test="./topic">
      <UL>
        <xsl:apply-templates />
      </UL>
    </xsl:if>
  </LI>
</xsl:template>
  
</xsl:stylesheet> 
