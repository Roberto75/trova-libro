<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output encoding="ISO-8859-1" indent="yes" omit-xml-declaration="yes"/>
	<xsl:param name="contesto"/>
  
  
  
	<xsl:template match="//Categorie">
	
      <ul class="list-group">
				<!--li>Roles:<xsl:value-of select="$roles"/></li-->
				<xsl:apply-templates select="Categoria" mode ="L1"/>
				<!--li><img src="images/tab_right.gif"></img></li-->
			</ul>
	
	</xsl:template>
	
  
  
	<xsl:template match="Categoria" mode="L1">
    <li class="list-group-item list-group-item-success" >
      <xsl:element name ="a">
        <xsl:attribute name ="href"><xsl:value-of select="$contesto"/>/Libri/Categoria/<xsl:value-of select="categoria_id"/></xsl:attribute>
        <xsl:attribute name ="title"><xsl:value-of select="nome"/></xsl:attribute>
        <xsl:value-of select="nome"/>
      </xsl:element>
    </li>
      <!--<ul class="categorie_L2"> -->
        <xsl:apply-templates select="Categorie/Categoria" mode="L2"/>
      <!-- </ul> -->
    
	</xsl:template>



  <xsl:template match="Categoria" mode="L2" >
    <li class="list-group-item list-group-item-primary " >
      <xsl:element name ="a">
        <xsl:attribute name ="href"><xsl:value-of select="$contesto"/>/Libri/Categoria/<xsl:value-of select="categoria_id"/></xsl:attribute>
        <xsl:attribute name ="title"><xsl:value-of select="nome"/></xsl:attribute>
        <xsl:attribute name ="class">ml-3 </xsl:attribute>
        <xsl:value-of select="nome"/>
      </xsl:element>
      <xsl:element name ="span">
        <xsl:attribute name ="class">badge badge-light badge-pill</xsl:attribute>
        <xsl:value-of select="COUNT_ANNUNCI"/>
      </xsl:element>
    </li>
    <!--<ul class="categorie_L3"> -->
      <xsl:apply-templates select="Categorie/Categoria" mode="L3"/>
    <!-- </ul>  -->
  </xsl:template>





  <xsl:template match="Categoria" mode="L3" >
    <li  class="list-group-item list-group-item-warning" >
      <xsl:element name ="a">
        <xsl:attribute name ="href"><xsl:value-of select="$contesto"/>/Libri/Categoria/<xsl:value-of select="categoria_id"/></xsl:attribute>
        <xsl:attribute name ="title"><xsl:value-of select="nome"/></xsl:attribute>
        <xsl:attribute name ="class">ml-5</xsl:attribute>
        <xsl:value-of select="nome"/>
        
        <xsl:element name ="span">
          <xsl:attribute name ="class">badge badge-light badge-pill</xsl:attribute>
          <xsl:value-of select="COUNT_ANNUNCI"/>
        </xsl:element>
      
      </xsl:element>
    </li>
  </xsl:template>




</xsl:stylesheet>
