@Code    
    ViewData("Title") = "CRM Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
 
    <script type="text/javascript" src="../../Scripts/sources/CRMSetup/CollectionSetup.js"></script>

    <div id="Welcome" data-role="content">
	    <div data-role="header" data-theme="d">
		    <h4>Overdue Penalty</h4>
	    </div><!-- /header -->

	    <div data-role="content">
            <div>
	            <h4> Overdue Penalty 1: </h4>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionFrom2">From :</label>
	            <input type="text" id="txtCollectionFrom2" style="width:50%"/>
                <label>&nbsp; Days Overdue</label>
            </div>
            
            <div data-role="fieldcontain">
	            <label for="txtCollectionTo2">To :</label>
	            <input type="text" id="txtCollectionTo2" style="width:50%"/>
                <label>&nbsp; Days Overdue</label>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionDecrease2">Incentive for overdue invoice is decreased by (%):</label>
	            <input type="text" id="txtCollectionDecrease2" style="width:50%"/>
            </div>
            <hr />

            <div>
	            <h4> Overdue Penalty 2: </h4>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionFrom3">From :</label>
	            <input type="text" id="txtCollectionFrom3" style="width:50%"/>
                <label>&nbsp; Days Overdue</label>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionTo3">To :</label>
	            <input type="text" id="txtCollectionTo3" style="width:50%"/>
                <label>&nbsp; Days Overdue</label>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionDecrease3">Incentive for overdue invoice is decreased by (%) :</label>
	            <input type="text" id="txtCollectionDecrease3" style="width:50%"/>
            </div>

            <hr />

            <div>
	            <h4> Overdue Penalty 3: </h4>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionFrom4">Above :</label>
	            <input type="text" id="txtCollectionFrom4" style="width:50%"/>
                <label>&nbsp; Days Overdue</label>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtCollectionDecrease3">Incentive for overdue invoice is decreased by (%) :</label>
	            <input type="text" id="txtCollectionDecrease4" style="width:50%"/>
            </div>

			<div style="text-align:center"> <a href="#" id="btnNext" data-role="button">Next</a> </div>

	    </div> <!-- /content -->	
    </div>           


