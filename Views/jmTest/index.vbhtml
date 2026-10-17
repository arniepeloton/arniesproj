@Code
    ViewData("Title") = "jQuery Mobile Test"
    Layout = "layout.vbhtml"
End Code

<ul data-role="listview" data-inset="true" data-filter="true">
	<li><a href="#">Acura</a></li>
	<li><a href="#">Audi</a></li>
	<li><a href="#">BMW</a></li>
	<li><a href="#">Cadillac</a></li>
	<li><a href="#">Ferrari</a></li>
</ul>

<form>
   <label for="slider-0">Input slider:</label>
   <input type="range" name="slider" id="slider-0" value="25" min="0" max="100"  />
</form>

<a href="#" data-role="button" data-icon="star">Star button</a>

<a href="jmTest/Page2" data-role="button" data-icon="star">Page 2</a>
<a href="jmTest/Page2" data-role="button" data-icon="star" data-rel="dialog">Page 2 dialog</a>
