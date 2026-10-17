
@RenderPage("Header.vbhtml")
@RenderPage("Sidebar.vbhtml")
<section id="content" class="container_12">    
    <div>             
        <ul class="breadcrumb" id="PageFlow">
                <li><a href="#" id="level0" style="visibility:hidden">Employees</a></li> 
				<li><a href="#" id="level1" style="visibility:hidden"></a></li>
				<li><a href="#" id="level2" style="visibility:hidden"></a></li>					
                <li><a href="#" id="level3" style="visibility:hidden"></a></li>					
                <li><a href="#" id="level4" style="visibility:hidden"></a></li>					
			</ul>            
    </div> 
    @RenderBody()
</section>
@RenderPage("Footer.vbhtml")