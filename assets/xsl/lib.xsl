<?xml version='1.0'?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                version='1.0'>

	<!--summary of powers of two-->
	<xsl:variable name="bit15" select="32768"/>
	<xsl:variable name="bit14" select="16384"/>
	<xsl:variable name="bit13" select="8192"/>
	<xsl:variable name="bit12" select="4096"/>
	<xsl:variable name="bit11" select="2048"/>
	<xsl:variable name="bit10" select="1024"/>
	<xsl:variable name="bit9"  select="512"/>
	<xsl:variable name="bit8"  select="256"/>
	<xsl:variable name="bit7"  select="128"/>
	<xsl:variable name="bit6"  select="64"/>
	<xsl:variable name="bit5"  select="32"/>
	<xsl:variable name="bit4"  select="16"/>
	<xsl:variable name="bit3"  select="8"/>
	<xsl:variable name="bit2"  select="4"/>
	<xsl:variable name="bit1"  select="2"/>
	<xsl:variable name="bit0"  select="1"/>

	<xsl:template name="bittest">
		<xsl:param name="decimalnumber"/>
		<xsl:param name="bit" select="1"/>
		<xsl:choose>
			<xsl:when test="( $decimalnumber mod ( $bit * 2 ) ) -
							( $decimalnumber mod ( $bit     ) )">1</xsl:when>
			<xsl:otherwise>0</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="DecimalToHex">
		<xsl:param name="number">0</xsl:param>

		<xsl:variable name="low">
			<xsl:value-of select="$number mod 16"/>
		</xsl:variable>

		<xsl:variable name="high">
			<xsl:value-of select="floor($number div 16)"/>
		</xsl:variable>

		<xsl:choose>
			<xsl:when test="$high &gt; 0">
			<xsl:call-template name="DecimalToHex">
				<xsl:with-param name="number">
				<xsl:value-of select="$high"/>
				</xsl:with-param>
			</xsl:call-template>  
			</xsl:when>
			<xsl:otherwise>
			<xsl:text>0x</xsl:text>
			</xsl:otherwise>
		</xsl:choose>  

		<xsl:choose>
			<xsl:when test="$low &lt; 10">
			<xsl:value-of select="$low"/>
			</xsl:when>
			<xsl:otherwise>
			<xsl:variable name="temp">
				<xsl:value-of select="$low - 10"/>
			</xsl:variable>

			<xsl:value-of select="translate($temp, '012345', 'ABCDEF')"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template> 


	
	<xsl:template name="HexToDecimal">
		<xsl:param name="value" select="'0'"/>
		<!-- the following paremeters are used only during recursion -->
		<xsl:param name="hex-power" select="number(1)"/>
		<xsl:param name="accum" select="number(0)"/>
		<!-- isolate last hex digit (and convert it to upper case) -->
		<xsl:variable name="hex-digit" select="translate(substring($value,string-length($value),1),'abcdef','ABCDEF')"/>
		<!-- check that hex digit is valid -->
		<xsl:choose>
			<xsl:when test="not(contains('0123456789ABCDEF',$hex-digit))">
				<!-- not a hex digit! -->
				<xsl:text>NaN</xsl:text>
			</xsl:when>
			<xsl:when test="string-length($hex-digit) = 0">
				<!-- unexpected end of hex string -->
				<xsl:text>0</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<!-- OK so far -->
				<xsl:variable name="remainder" select="substring($value,1,string-length($value)-1)"/>
				<xsl:variable name="this-digit-value" select="string-length(substring-before('0123456789ABCDEF',$hex-digit)) * $hex-power"/>
				<!-- determine whether this is the end of the hex string -->
				<xsl:choose>
					<xsl:when test="string-length($remainder) = 0">
						<!-- end - output final result -->
						<xsl:value-of select="$accum + $this-digit-value"/>
					</xsl:when>
					<xsl:otherwise>
						<!-- recurse to self for next digit -->
						<xsl:call-template name="HexToDecimal">
							<xsl:with-param name="value" select="$remainder"/>
							<xsl:with-param name="hex-power" select="$hex-power * 16"/>
							<xsl:with-param name="accum" select="$accum + $this-digit-value"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="remove-trailing">
		<xsl:param name="text"/>
		<xsl:param name="chars"/>

		<xsl:choose>

			<xsl:when test="substring($text,string-length($text) - string-length($chars) + 1) = $chars" >
				<xsl:value-of select="substring($text,1,string-length($text) - string-length($chars))"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$text"/>
			</xsl:otherwise>
		</xsl:choose>
	
	</xsl:template>  
</xsl:stylesheet>

