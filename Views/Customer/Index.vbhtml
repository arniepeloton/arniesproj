@Code
    'ViewData("Title") = "Customer"
    'Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<div data-role="page">
    <script>
        require(['/Scripts3/Browser/LibraryCustomers.js',
            '/Scripts3/Customer.js'
        ]);
    </script>
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Customer</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Customer:</legend>
                <div data-role="controlgroup">
                    <p id="pCompanyName">
                    </p>
                    <button id="butCompany">Browse...</button>
                </div>
            </fieldset>
        </div>
        <button id="butOk">Ok</button>
    </div>
</div>

<div id="divtemp" data-role="page">
    <script>
        console.log('divtemp');
    </script>
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>asdasdasdasd</h1>
    </div>
    <div data-role="content">
        
    </div>
</div>

@*<div id="pageResults" data-role="page">

    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Customer</h1>
        <a href="" class="ui-btn-right" data-icon="arrow-r">Ok</a>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Font styling:</legend>
                <div data-role="controlgroup">eret2454<ul><li>sdadasdasd</li><li>asdasdasfsdf</li></ul></div>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <label for="search">
                Search Input:<span>*</span></label>
            <input type="search" name="password" id="search" value="" />
        </div>
        <div data-role="fieldcontain">
            <label for="slider2">
                Flip switch:</label>
            <select name="slider2" id="slider2" data-role="slider">
                <option value="off">Off</option>
                <option value="on">On</option>
            </select>
        </div>
        <div data-role="fieldcontain">
            <label for="slider">
                Slider:</label>
            <input type="range" name="slider" id="slider" value="50" min="0" max="100" data-highlight="true" />
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Choose as many snacks as you'd like:</legend>
                <input type="checkbox" name="checkbox-1a" id="checkbox-1a" class="custom" />
                <label for="checkbox-1a">
                    Cheetos</label>
                <input type="checkbox" name="checkbox-2a" id="checkbox-2a" class="custom" />
                <label for="checkbox-2a">
                    Doritos</label>
                <input type="checkbox" name="checkbox-3a" id="checkbox-3a" class="custom" />
                <label for="checkbox-3a">
                    Fritos</label>
                <input type="checkbox" name="checkbox-4a" id="checkbox-4a" class="custom" />
                <label for="checkbox-4a">
                    Sun Chips</label>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Font styling:</legend>
                <input type="checkbox" name="checkbox-6" id="checkbox-6" class="custom" />
                <label for="checkbox-6">
                    b</label>
                <input type="checkbox" name="checkbox-7" id="checkbox-7" class="custom" />
                <label for="checkbox-7">
                    <em>i</em></label>
                <input type="checkbox" name="checkbox-8" id="checkbox-8" class="custom" />
                <label for="checkbox-8">
                    u</label>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Choose a pet:</legend>
                <input type="radio" name="radio-choice-1" id="radio-choice-1" value="choice-1" checked="checked" />
                <label for="radio-choice-1">
                    Cat</label>
                <input type="radio" name="radio-choice-1" id="radio-choice-2" value="choice-2" />
                <label for="radio-choice-2">
                    Dog</label>
                <input type="radio" name="radio-choice-1" id="radio-choice-3" value="choice-3" />
                <label for="radio-choice-3">
                    Hamster</label>
                <input type="radio" name="radio-choice-1" id="radio-choice-4" value="choice-4" />
                <label for="radio-choice-4">
                    Lizard</label>
            </fieldset>
        </div>
    </div>
</div>*@ 