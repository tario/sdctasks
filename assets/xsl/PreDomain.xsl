<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xlink="http://www.w3.org/1999/xlink">
  <xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes"/>

  <xsl:template match="ppdoc">
    <xsl:copy>
      <xsl:apply-templates select="*"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="properties">
    <xsl:copy-of select="." />
  </xsl:template>

  <!-- match the body node -->
  <xsl:template match="body">

    <xsl:copy>

      <!-- Process all nodes that come before the first Chapter, Topic, or Section -->
      <xsl:apply-templates select="node()[not(self::chapter | self::topic | self::section) and
       not(preceding::chapter) and not(preceding::topic) and not(preceding::section)]"/>

      <xsl:choose>
        <xsl:when test="count(chapter) &gt; 0">
          <xsl:apply-templates select="chapter" />
        </xsl:when>
        <xsl:when test="count(topic) &gt; 0">
          <xsl:apply-templates select="topic" />
        </xsl:when>
        <xsl:when test="count(section) &gt; 0">
          <xsl:apply-templates select="section" />
        </xsl:when>
      </xsl:choose>

    </xsl:copy>

  </xsl:template>

  <xsl:key name="next-headings" match="section"
           use="generate-id(preceding-sibling::*[self::chapter or self::topic][1])" />
  
  <xsl:key name="next-headings" match="topic"
           use="generate-id(preceding-sibling::chapter[1])" />

  <xsl:key name="immediate-nodes"
           match="node()[not(self::chapter | self::topic | self::section)]"
           use="generate-id(preceding-sibling::*[self::chapter or self::topic or
                                               self::section][1])" />

  <xsl:template match="chapter | topic | section">
    <xsl:copy>
      <xsl:attribute name="id">
        <xsl:value-of select="position()"/>
      </xsl:attribute>
      <title><xsl:apply-templates /></title>
      <contents>
        <xsl:apply-templates select="key('immediate-nodes', generate-id())" />
        <xsl:apply-templates select="key('next-headings', generate-id())" />
      </contents>
    </xsl:copy>
  </xsl:template>
  
  <xsl:template match="node()">
    <xsl:copy-of select="." />
  </xsl:template>

</xsl:stylesheet>