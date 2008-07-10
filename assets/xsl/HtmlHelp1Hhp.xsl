<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output encoding="utf-8" method="text"/>
  <xsl:variable name="outputPath">html/</xsl:variable>
  
  <xsl:param name="configFile"/>
  <xsl:variable name="config" select="document($configFile)/options"/>
  <xsl:variable name="title" select="$config/title"/>
  <xsl:variable name="projectName" select="$config/ProjectName"/>
  <xsl:variable name="prefix" select="$config/prefix" />

  <!--Create document structure-->
<xsl:template match="/">[OPTIONS]
Auto Index=Yes
Binary TOC=Yes
Compatibility=1.1 or later
Compiled file=<xsl:value-of select="$projectName"/>.chm
Contents file=Toc.hhc
Default topic=<xsl:value-of select="$outputPath"/><xsl:value-of select="concat($prefix, masterToc/toc/topic[1]/@id)" />.html
Full-text search=Yes
Flat=No
Language=0x409 English (United States)
Title=<xsl:value-of select="$title"/>

[FILES]
<xsl:apply-templates select="masterToc/toc//topic"/>
</xsl:template>

<xsl:template match="topic">
<xsl:value-of select="$outputPath"/><xsl:value-of select="concat($prefix, @id)" />.html
</xsl:template>
</xsl:stylesheet> 
