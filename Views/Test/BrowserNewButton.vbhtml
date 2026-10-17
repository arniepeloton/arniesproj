@Code
    ViewData("Title") = "Browser New Button"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script src="/Scripts3/Browser/common.js"></script>
<script src="/Scripts3/Browser/LibraryCustomers.js"></script>
<script src="/Scripts3/Test/BrowserNewButton.js"></script>

<h1>
    Browser New Button</h1>
<button id="butBrowse">Browse</button>

<h3>I'm a header</h3>
			<p>I'm the collapsible content. By default I'm open and displayed on the page, but you can click the header to hide me.</p>
			
			<div data-role="collapsible">
				<h3>I'm a nested collapsible with a child collapsible</h3>
				<p>I'm a child collapsible.</p>
					<div data-role="collapsible">
						<h3>Nested inside again.</h3>
						<p>Three levels deep now.</p>
					</div><!-- /section 1A -->
			</div><!-- /section 1 -->

<div data-role="collapsible" data-collapsed="false" data-content-theme="d">
			<h3>I'm a header</h3>
			<p>I'm the collapsible content. By default I'm open and displayed on the page, but you can click the header to hide me.</p>
			
			<div data-role="collapsible">
				<h3>I'm a nested collapsible with a child collapsible</h3>
				<p>I'm a child collapsible.</p>
					<div data-role="collapsible">
						<h3>Nested inside again.</h3>
						<p>Three levels deep now.</p>
					</div><!-- /section 1A -->
			</div><!-- /section 1 -->

			
			<div data-role="collapsible">
				<h3>Section 3: Form elements</h3>
				<form action="#" method="get">
					<div data-role="fieldcontain">
						<label for="textarea">Textarea:</label>
						<textarea cols="40" rows="8" name="textarea" id="textarea"></textarea>
					</div>
					<div data-role="fieldcontain">
						<label for="slider">Input slider:</label>
					 	<input type="range" name="slider" id="slider" value="0" min="0" max="100"  />
					</div>
					<fieldset class="ui-grid-a">
							<div class="ui-block-a"><button type="submit">Cancel</button></div>
							<div class="ui-block-b"><button type="submit">Submit</button></div>	   
				    </fieldset>
				</form>
			</div><!-- /section 2 -->

			<div data-role="collapsible">
				<h3>Section 4: Collapsed list</h3>
				<p>Here is an inset list:</p>
				<ul data-role="listview" data-inset="true">
					<li><a href="index.html">Acura</a></li>
					<li><a href="index.html">Audi</a></li>
					<li><a href="index.html">BMW</a></li>
					<li><a href="index.html">Cadillac</a></li>
					<li><a href="index.html">Chrysler</a></li>
					<li><a href="index.html">Dodge</a></li>
					<li><a href="index.html">Ferrari</a></li>
					<li><a href="index.html">Ford</a></li>
				</ul>
			</div><!-- /section 3 -->
		</div>

@Section OtherPages
    <div id="pageCustomer" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>Add/Edit Customer</h1>
        </div>
        <div data-role="content">
            <button id="butBackToBrowse">Go back to browser</button>
            <button id="butBackToPage">Go back to page</button>
        </div>
    </div>
End Section
