<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:dt="uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"
                xmlns:xlink="http://www.w3.org/1999/xlink"
                xmlns:exsl="http://exslt.org/common"
                xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
                exclude-result-prefixes="exsl"
                >

  <xsl:output method="html" version="4.01" indent="yes" encoding="utf-8" standalone="yes"/>

  <xsl:param name="configFile" />

  <xsl:include href="html.xsl"/>
  <xsl:include href="Languages.xsl" />

  <xsl:variable name="up" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:variable name="lo" select="'abcdefghijklmnopqrstuvwxyz'"/>

  <!-- Configuration settings -->
  <!--<xsl:variable name="config" select="document('ppdocConfig.xml')/options"/>-->
  <xsl:variable name="config" select="document($configFile)/options"/>

  <!-- Topics related to this topic that should show in the related topics section  -->
  <xsl:variable name="relatedTopicsSection" select="boolean(string-length(/topic/relatedTopics[normalize-space(.)]) > 0)"/>

  <xsl:key name="bookmarks" match="bookmark" use="@name" />

  <!--<xsl:strip-space elements="hlink"/>-->
  <xsl:strip-space elements="*"/>

  <xsl:template match="*[@xlink:type = 'simple' and @xlink:href]/text()">
    <xsl:value-of select="normalize-space(.)"/>
  </xsl:template>
  <xsl:template match="i/text()">
    <xsl:value-of select="normalize-space(.)"/>
  </xsl:template>
  <xsl:template match="u/text()">
    <xsl:value-of select="normalize-space(.)"/>
  </xsl:template>

  <!-- =============================================================
     Match the root node
     ============================================================= -->

  <xsl:template match="ppdoc">
    <!-- Process document contents -->
    <xsl:apply-templates select="*" />
  </xsl:template>

  <xsl:template match="properties">
  </xsl:template>

  <xsl:template match="chapter | topic">
    <html xmlns:MSHelp="http://msdn.microsoft.com/mshelp" DIR="LTR">
      <xsl:apply-templates select="title" mode="topic"/>
      <xsl:apply-templates select="contents"  mode="topic"/>
    </html>
  </xsl:template>

  <xsl:template match="title" mode="topic">
    <head>
      <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
      <META NAME="save" CONTENT="history"/>
      <xml>
        <MSHelp:TOCTitle>
          <xsl:attribute name="Title">
            <xsl:value-of select="."/>
          </xsl:attribute>
        </MSHelp:TOCTitle>
        <MSHelp:RLTitle>
          <xsl:attribute name="Title">
            <xsl:value-of select="."/>
          </xsl:attribute>
        </MSHelp:RLTitle>
        <xsl:copy-of select="$config/xml/*"/>
        <xsl:if test="string-length(normalize-space(/ppdoc/properties/keywords)) &gt; 0">
          <xsl:call-template name="keywords">
            <xsl:with-param name="str" select="normalize-space(/ppdoc/properties/keywords)"/>
          </xsl:call-template>
        </xsl:if>
      </xml>

      <title>
        <xsl:value-of select="."/>
      </title>

      <xsl:for-each select="$config/stylesheet">
        <link rel="stylesheet" type="text/css" href="{.}">
        </link>
      </xsl:for-each>
      <xsl:for-each select="$config/script">
        <script src="{.}">
          <xsl:text> </xsl:text>
        </script>
      </xsl:for-each>
    </head>
  </xsl:template>

  <xsl:template match="contents" mode="topic">
    <body>
      <xsl:call-template name="header" />

      <div id="mainSection">
        <div id="mainBody">
          <div id="allHistory" class="saveHistory" onsave="saveAll()" onload="loadAll()"></div>
          <xsl:call-template name="prereleasetext"/>
          <xsl:apply-templates select="*" />
        </div>

        <xsl:call-template name="footer" />

      </div>
    </body>
  </xsl:template>

  <xsl:template name="keywords">
    <xsl:param name="str"/>
    <xsl:choose>
      <xsl:when test="contains($str,' ')">
        <MSHelp:Keyword Index="F" Term="{substring-before($str,' ')}" />
        <xsl:text>&#xA0;</xsl:text>
        <xsl:call-template name="keywords">
          <xsl:with-param name="str" select="substring-after($str,' ')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <MSHelp:Keyword Index="F" Term="{$str}" />
        <xsl:text>&#xA0;</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="header">
    <input type="hidden" id="userDataCache" class="userDataStyle"> </input>
    <input type="hidden" id="hiddenScrollOffset"> </input>

    <!-- IMAGES -->
    <img id="dropDownImage" style="display:none; height:0; width:0;" alt="DropDown image" src="{$config/imagespath}/drpdown.gif"/>
    <img id="dropDownHoverImage" style="display:none; height:0; width:0;" alt="DropDownHover image" src="{$config/imagespath}/drpdown_orange.gif"/>
    <img id="collapseImage" style="display:none; height:0; width:0;" alt="Collapse image" src="{$config/imagespath}/collapse_all.gif"/>
    <img id="expandImage" style="display:none; height:0; width:0;" alt="Expand image" src="{$config/imagespath}/expand_all.gif"/>
    <img id="collapseAllImage" style="display:none; height:0; width:0;" alt="CollapseAll image" src="{$config/imagespath}/collall.gif"/>
    <img id="expandAllImage" style="display:none; height:0; width:0;" alt="ExpandAll image" src="{$config/imagespath}/expall.gif"/>
    <img id="copyImage" style="display:none; height:0; width:0;" alt="Copy image" src="{$config/imagespath}/copycode.gif"/>
    <img id="copyHoverImage" style="display:none; height:0; width:0;" alt="CopyHover image" src="{$config/imagespath}/copycodeHighlight.gif"/>
    <!-- IMAGES -->

    <div id="header">
      <table width="100%" id="topTable">
        <tr id="headerTableRow1">
          <td align="left">
            <span id="runningHeaderText">
              <xsl:value-of select="$config/header"/>
            </span>
          </td>
        </tr>
        <tr id="headerTableRow2">
          <td align="left">
            <span id="nsrTitle">
              <xsl:value-of select="../title"/>
            </span>
          </td>
        </tr>
        <tr id="headerTableRow3">
          <td>
            <xsl:choose>
              <xsl:when test="$relatedTopicsSection">
                <a href="#seeAlsoNoToggle" onclick="OpenSection(seeAlsoNoToggle)">
                  <includeSSC item="SeeAlso"/>
                </a>
              </xsl:when>
            </xsl:choose>
          </td>
        </tr>
      </table>
      <table width="100%" id="bottomTable" cellspacing="0" cellpadding="0">
        <tr>
          <td>
            <span onclick="ExpandCollapseAll(toggleAllImage)" style="cursor:default;" onkeypress="ExpandCollapseAll_CheckKey(toggleAllImage)" tabindex="0">
              <xsl:choose>
                <xsl:when test="normalize-space(translate($config/collapsiblesections, $up, $lo)) = 'false'">
                  <xsl:attribute name="style">cursor:default;display:none;</xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:attribute name="style">cursor:default;</xsl:attribute>
                </xsl:otherwise>
              </xsl:choose>
              <img ID="toggleAllImage" class="toggleAll" alt="CollapseAll image" src="{$config/imagespath}/collall.gif"/>
              <label id="collapseAllLabel" for="toggleAllImage" style="display: none;">Collapse All</label>
              <label id="expandAllLabel" for="toggleAllImage" style="display: none;">Expand All</label>
              <xsl:text>&#160;</xsl:text>
            </span>
            <span id="languageFilterToolTip" onmouseover="languageFilterImage.src=dropDownHoverImage.src;" onmouseout="languageFilterImage.src=dropDownImage.src;" tabindex="0">
              <xsl:choose>
                <xsl:when test="count($config/languages/language) &gt; 1">
                  <xsl:attribute name="style">cursor:default;</xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:attribute name="style">cursor:default;display:none;</xsl:attribute>
                </xsl:otherwise>
              </xsl:choose>
              <img id="languageFilterImage" alt="DropDown image" src="{$config/imagespath}/drpdown.gif"/>
              <label id="showAllLabel" for="languageFilterImage" style="display: none;">Language Filter: All</label>
              <label id="multipleLabel" for="languageFilterImage" style="display: none;">Language Filter: Multiple</label>
              <label id="vbLabel" for="languageFilterImage" style="display: none;">Language Filter: Visual Basic</label>
              <label id="csLabel" for="languageFilterImage" style="display: none;">Language Filter: C#</label>
            </span>
          </td>
        </tr>
      </table>
      <xsl:if test="count($config/languages/language) &gt; 1">
        <xsl:call-template name="LanguageDropdown"/>
      </xsl:if>

    </div>
  </xsl:template>

  <xsl:template name="footer">
    <div id="footer">
      <div class="footerLine">
        <xsl:apply-templates select="$config/footer"/>
        <!-- img alt="Footer image" width="100%" height="3px" src="{$config/imagespath}/footer.gif" / -->
      </div>
      <xsl:apply-templates select="$config/feedback"/>
      <p></p>
      <xsl:apply-templates select="$config/copyright"/>
    </div>
  </xsl:template>

  <!-- =============================================================
     Sections
     ============================================================= -->
  <xsl:template match="section">

    <xsl:variable name="sectionNumber">
      <xsl:value-of select="count(preceding-sibling::section)"/>
    </xsl:variable>

    <h1 class="heading">
      <span onclick="ExpandCollapse(sectionToggle{$sectionNumber})" style="cursor:default;" onkeypress="ExpandCollapse_CheckKey(sectionToggle{$sectionNumber})" tabindex="0">
        <img id="sectionToggle{$sectionNumber}" onload="OnLoadImage()" class="toggle" name="toggleSwitch" src="../local/exp.gif"></img>
        <xsl:value-of select="title"/>
      </span>
    </h1>

    <div id="sectionSection{$sectionNumber}" class="section" name="collapseableSection" style="display: none;">
      <xsl:apply-templates select="contents"/>
    </div>
  </xsl:template>

  <!-- =============================================================
     Headings
     ============================================================= -->

  <xsl:template match="h1">
    <xsl:element name="{name(.)}">
      <xsl:attribute name="class">heading</xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="h2">
    <h3>
      <xsl:attribute name="class">subheading</xsl:attribute>
      <xsl:apply-templates/>
    </h3>
  </xsl:template>

  <xsl:template match="h3 | h4">
    <xsl:element name="{name(.)}">
      <xsl:attribute name="class">subheading</xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <!-- =============================================================
     Procedures and steps
     ============================================================= -->

  <xsl:template match="procedure">
    <xsl:apply-templates select="title" />
    <xsl:apply-templates select="steps"/>
  </xsl:template>

  <xsl:template match="procedure/title">
    <p>
      <b>
        <xsl:value-of select="."/>
      </b>
    </p>
  </xsl:template>

  <xsl:template match="steps[@style='ol']">
    <ol>
      <xsl:apply-templates select="step"/>
      &#160;
    </ol>
  </xsl:template>

  <xsl:template match="steps[@style='ul']">
    <ul>
      <xsl:apply-templates select="step"/>
      &#160;
    </ul>
  </xsl:template>

  <xsl:template match="step">
    <li>
      <xsl:apply-templates select="title"/>
      <xsl:apply-templates select="contents"/>
    </li>
  </xsl:template>

  <!-- =============================================================
     Code
     ============================================================= -->

  <xsl:template match="code">
    <xsl:variable name="codeLang">
      <xsl:choose>
        <xsl:when test="@language = 'vbs'">
          <xsl:text>VBScript</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'vb' or @language = 'vb#'  or @language = 'VB' or @language = '[Visual Basic]' or 
                        @language = 'Visual Basic' or @language = 'Visual&#160;Basic' or @language = '[Visual&#160;Basic]'" >
          <xsl:text>VisualBasicUsage</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'c#' or @language = 'cs' or @language = 'C#' or @language = '[C#]'" >
          <xsl:text>CSharp</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'cpp' or @language = 'cpp#' or @language = 'c' or @language = 'c++' or @language = 'C++'" >
          <xsl:text>ManagedCPlusPlus</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'j#' or @language = 'jsharp'">
          <xsl:text>JSharp</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'js' or @language = 'jscript#' or @language = 'jscript' or @language = 'JScript'">
          <xsl:text>JScript</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'xml'">
          <xsl:text>xml</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'html'">
          <xsl:text>html</xsl:text>
        </xsl:when>
        <xsl:when test="@language = 'vb-c#'">
          <xsl:text>visualbasicANDcsharp</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>other</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:choose>
      <xsl:when test="count(line) = 0">
      </xsl:when>

      <xsl:otherwise>
        <div class="code">
          <span codeLanguage="{$codeLang}">
            <table width="100%" cellspacing="0" cellpadding="0">
              <tr>
                <xsl:if test="@language != 'other'">
                  <th>
                    <xsl:value-of select="@language" />
                  </th>  
                </xsl:if>                
                <th>
                  <span class="copyCode" onclick="CopyCode(this)" onkeypress="CopyCode_CheckKey(this)" onmouseover="ChangeCopyCodeIcon(this)" onfocusin="ChangeCopyCodeIcon(this)" onmouseout="ChangeCopyCodeIcon(this)" onfocusout="ChangeCopyCodeIcon(this)" tabindex="0">
                    <img class="copyCodeImage" name="ccImage" align="absmiddle" src="{$config/imagespath}/copycode.gif"></img>Copy Code
                  </span>
                </th>
              </tr>
              <tr>
                <td colspan="2">
                  <pre>
                    <xsl:apply-templates select="line"/>
                  </pre>
                </td>
              </tr>
            </table>
          </span>
        </div>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="line">
    <xsl:if test="name(child::node()) = 'b'">
    <b><xsl:value-of select="."/></b>
    <xsl:text>&#xD;</xsl:text>
    </xsl:if>
    <xsl:if test="not(name(child::node()) = 'b')">
    <xsl:value-of select="."/>
    <xsl:text>&#xD;</xsl:text>     
    </xsl:if>
  </xsl:template>

  <!-- =============================================================
     Note
     ============================================================= -->

  <xsl:template match="note">
    <div class="alert">
      <table width="100%" cellspacing="0" cellpadding="0">
        <tr>
          <th align="left">
            <img class="note" src="../local/note.gif"></img>
            <xsl:choose>
              <xsl:when test="title">
                <!--
							<xsl:call-template name="remove-trailing" >
								<xsl:with-param name="text" select="title" />
								<xsl:with-param name="chars" select="':'" />
							</xsl:call-template>
							-->
                <xsl:apply-templates select="title" />
              </xsl:when>
              <xsl:otherwise>Note </xsl:otherwise>
            </xsl:choose>
          </th>
        </tr>
        <tr>
          <td>
            <xsl:apply-templates select="text" />
          </td>
        </tr>
      </table>
      <p></p>
    </div>
  </xsl:template>

  <!-- =============================================================
     Lists
     ============================================================= -->

  <xsl:template match="ol | ul">
    <xsl:copy>
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>

  <!-- =============================================================
     Tables
     ============================================================= -->

  <xsl:template match="tablecaption">
    <h4 class="subheading">
      <xsl:apply-templates/>
    </h4>
  </xsl:template>

  <!-- match the  table tag -->
  <xsl:template match="table">
    <table>
      <xsl:attribute name="style">
        <xsl:value-of select="@style" />
      </xsl:attribute>
      <xsl:apply-templates />
    </table>
  </xsl:template>


  <!-- match the table row <w:tr> tag -->
  <xsl:template match="tr">
    <tr valign="top">
      <xsl:apply-templates />
    </tr>
  </xsl:template>

  <!-- match the table heading tag -->
  <xsl:template match="th">
    <xsl:if test="@colspan != ''">
      <th colspan="{@colspan}">
        <xsl:apply-templates />
      </th>
    </xsl:if>
    <xsl:if test="@rowspan != ''">
      <th rowspan="{@rowspan}">
        <xsl:apply-templates />
      </th>
    </xsl:if>
    <xsl:if test="not(@colspan) and not(@rowspan)">
      <th>
        <xsl:apply-templates />
      </th>
    </xsl:if>
  </xsl:template>

  <!-- match the table cell <td> tag -->
  <xsl:template match="td">
    <xsl:if test="@colspan != ''">
      <td colspan="{@colspan}">
        <xsl:choose>
          <xsl:when test="string-length(normalize-space(.)) > 0">
            <xsl:apply-templates />
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="dingbat">
              <xsl:with-param name="dingbat" select="'nbsp'"/>
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </td>
    </xsl:if>
    <xsl:if test="@rowspan != ''">
      <td rowspan="{@rowspan}">
        <xsl:choose>
          <xsl:when test="string-length(normalize-space(.)) > 0">
            <xsl:apply-templates />
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="dingbat">
              <xsl:with-param name="dingbat" select="'nbsp'"/>
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </td>
    </xsl:if>
    <xsl:if test="not(@colspan) and not(@rowspan)">
      <td>
        <xsl:choose>
          <xsl:when test="string-length(normalize-space(.)) > 0">
            <xsl:apply-templates />
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="dingbat">
              <xsl:with-param name="dingbat" select="'nbsp'"/>
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </td>
    </xsl:if>
  </xsl:template>

  <!-- =============================================================
     Checklists
     ============================================================= -->

  <xsl:template match="checklist">
    <xsl:apply-templates select="caption"/>

    <table class="ppChecklist">
      <col width="7%"/>
      <col width="93%"/>
      <xsl:apply-templates select="columnheaders"/>
      <xsl:apply-templates select="item"/>
    </table>
  </xsl:template>

  <xsl:template match="caption">
    <h4 class="subheading">
      <xsl:apply-templates/>
    </h4>
  </xsl:template>

  <xsl:template match="columnheaders">
    <tr valign="top">
      <xsl:apply-templates />
    </tr>
  </xsl:template>

  <xsl:template match="header">
    <th>
      <xsl:apply-templates />
    </th>
  </xsl:template>

  <!-- match the table row <item> tag -->
  <xsl:template match="checklist/item">
    <tr valign="">
      <td align="middle">
        <xsl:call-template name="dingbat">
          <xsl:with-param name="dingbat" select="'checkbox'"/>
        </xsl:call-template>
      </td>
      <td>
        <xsl:apply-templates />
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="p">
    <p>
      <xsl:apply-templates/>
    </p>
  </xsl:template>

  <xsl:template match="br">
    <br/>
  </xsl:template>

  <xsl:template match="b | i | u" priority="3" >
    <xsl:variable name="elementName" select="name(.)"/>
    <xsl:if test="parent::node()/child::node()[1] = .">
      <xsl:if test="not($elementName = 'u')">
        <xsl:text>.d</xsl:text>
      </xsl:if>
    </xsl:if>
    <!-- ouput the current element -->
    <xsl:element name="{$elementName}">
      <!-- output the content -->
      <xsl:if test="text() = false() and node() = false()">
        <xsl:text>&#160;</xsl:text>
      </xsl:if>
      <xsl:if test="not(text() = false() and node() = false())">
        <xsl:apply-templates />
      </xsl:if>

    </xsl:element>
  </xsl:template>

  <xsl:template match="b1 | i1 | u1" priority="2" >

    <xsl:variable name="elementName" select="name(.)"/>

    <!-- only if the preceeding element is not the same as the current element-->
    <xsl:if test="not(local-name(preceding-sibling::node()[1])=string($elementName))">

      <!-- ouput the current element -->
      <xsl:element name="{$elementName}">
        <!-- output the content -->
        <xsl:apply-templates />

        <!-- if the following element matches the current element -->
        <xsl:if test="name(following-sibling::node()[1])=string($elementName)">
          <xsl:apply-templates select="following-sibling::node()[1]" mode="following"/>
        </xsl:if>

      </xsl:element>
    </xsl:if>
  </xsl:template>

  <!-- run of formatting tags, eg tags without attributes -->
  <!-- match either <b> or <i> or <u>-->
  <xsl:template match="b | i | u" priority="2" >

    <xsl:variable name="elementName" select="name()"/>

    <!-- only if the preceeding element is not the same as the current element-->
    <xsl:if test="not(local-name(preceding-sibling::*[1])=name())">

      <!-- ouput the current element -->
      <xsl:element name="{$elementName}">
        <!-- output the content -->
        <xsl:apply-templates />

        <xsl:call-template name="makegroup">
          <xsl:with-param name="name" select="name()" />
          <xsl:with-param name="list" select="following-sibling::node()" />
        </xsl:call-template>

      </xsl:element>
    </xsl:if>
  </xsl:template>

  <xsl:template name="makegroup">
    <xsl:param name="name" />
    <xsl:param name="list" />

    <xsl:if test="$name = name($list[1])">
      <!--<xsl:value-of select="$list[1]"/>-->
      <!--<xsl:apply-templates select="$list[1]"/>-->
      <xsl:copy-of select="$list[1]"/>

      <xsl:call-template name="makegroup">
        <xsl:with-param name="name" select="$name" />
        <xsl:with-param name="list" select="$list[position() &gt; 1]" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>


  <!-- Only invoked when a mode of "following" is specified -->
  <xsl:template match="b2 | i2 | u2" mode="following" >

    <xsl:variable name="elementName" select="name(.)"/>
    <!-- output the content -->
    <xsl:apply-templates/>

    <!-- If the following element matches the current element, recurse -->
    <xsl:if test="name(following-sibling::node()[1])=string($elementName)">
      <xsl:apply-templates select="following-sibling::node()[1]" mode="following"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="tab">
    <xsl:variable name="spaces">
      <xsl:call-template name="GetTabSpace"/>
    </xsl:variable>
    <xsl:call-template name="indent">
      <xsl:with-param name="n" select="$spaces" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="GetTabSpace">
    <xsl:choose>
      <xsl:when test="count($config/options/tabspace) > 0">
        <xsl:value-of select="$config/options/tabspace"/>
      </xsl:when>
      <xsl:otherwise>4</xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <!-- =============================================================
     Hyperlink support; 
          links to external sites are represented with <w:hlink> 
          nodes within a paragraph;  <w:hlink> nodes may also
          represent links to internal positions within the document
          if the w:bookmark attribute is present.  Bookmarks are
          represented with <aml:annotation> tags, so we'll look 
          for those too.
     ============================================================= -->
  <!-- match hlink nodes within a paragraph -->
  <xsl:template match="a">
    <xsl:copy-of select="."/>
  </xsl:template>

  <xsl:template match="*[@xlink:type = 'simple' and @xlink:href]">
    <xsl:variable name="xref">
      <xsl:call-template name="GetHyperlinkTarget">
        <xsl:with-param name="targetName" select="@xlink:href" />
      </xsl:call-template>
    </xsl:variable>

    <xsl:variable name="docset" select="concat($config/xml/MSHelp:Attr[@Name='DocSet']/@Value, '_')" />
    <xsl:variable name="finalDocSet" select="concat($docset, $xref)" />

    <xsl:if test="not(starts-with($xref, 'http'))">
      <a href="{$finalDocSet}">
        <xsl:apply-templates />
      </a>
    </xsl:if>
    <xsl:if test="starts-with($xref, 'http')">
      <a href="{$xref}">
        <xsl:apply-templates />
      </a>
    </xsl:if>
  </xsl:template>

  <xsl:template name="GetHyperlinkTarget">
    <xsl:param name="targetName" />

    <xsl:variable name="result">
      <xsl:choose>
        <xsl:when test="contains(translate($targetName, $up, $lo), '.doc')">
          <xsl:value-of select="concat(substring($targetName, 1, string-length($targetName)-4), '.htm')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$targetName" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:value-of select="$result" />
  </xsl:template>

  <!-- match the bookmark tag -->
  <xsl:template match="bookmark">
    <!-- use the name attribute to identify the bookmark name -->
    <!-- place an anchor link here with that name -->
    <a name="{@name}" ></a>
  </xsl:template>

  <!-- =============================================================
     Images
     ============================================================= -->

  <xsl:template match="img">
    <xsl:variable name="correctedSrc">
      <xsl:if test="substring-after(@src,'images/')!=''">
        ../images/<xsl:value-of select="substring-after(@src,'images/')"/>
      </xsl:if>
      <xsl:if test="substring-after(@src,'images/')=''">
        <xsl:value-of select="@src"/>
      </xsl:if>
    </xsl:variable>

    <img>
      <xsl:attribute name="src">
        <xsl:value-of select="normalize-space($correctedSrc)"/>
      </xsl:attribute>
      <xsl:for-each select="@*[local-name()!='src']">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
    </img>
  </xsl:template>

  <xsl:template match="figurenumber">
    <p>
      <!--<xsl:attribute name="class">
        <xsl:value-of select="@class"/>
      </xsl:attribute>-->
      <b>
        <xsl:value-of select="."/>
      </b>
      <br/>
      <xsl:apply-templates select="following-sibling::*[1][self::figurecaption]" mode="explicit"/>
    </p>
  </xsl:template>

  <xsl:template match="figurecaption">
    <xsl:if test="not(preceding-sibling::*[1][self::figurenumber])">
      <p class="label">
        <xsl:apply-templates select="." mode="explicit"/>
      </p>
    </xsl:if>
  </xsl:template>

  <xsl:template match="figurecaption" mode="explicit">
    <i>
      <xsl:value-of select="."/>
    </i>
  </xsl:template>

  <xsl:template name="prereleasetext">
    <xsl:if test="string-length($config/preReleaseText[normalize-space(.)]) > 0">
      <font color="DarkGray">
        <xsl:value-of select="$config/preReleaseText"/>
      </font>
      <p />
    </xsl:if>
  </xsl:template>

  <!-- =============================================================
     Show Me 
     ============================================================= -->

  <xsl:template match="showme">
    <xsl:variable name="video" select="." />
    <p>
      <a onclick="parser('{$video}')">
        <img style="cursor:hand;" src="{$config/imagespath}/ShowMe.png" />
      </a>
    </p>
  </xsl:template>

  <!-- =============================================================
     Blanck Space
     ============================================================= -->

  <xsl:template match="space">
    <xsl:text disable-output-escaping="yes">&#160;</xsl:text>
  </xsl:template>

</xsl:stylesheet>
