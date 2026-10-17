@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleDTRPIN.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleName" data-role="page">

    <div id"divHeader" data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeopleInfoPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>DTR PIN</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtPIN1">Enter PIN: <span style="color: #FF0000;" >*</span></label>
            <input type="password" id="txtPIN1"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtPIN2">Re-Enter PIN: <span style="color: #FF0000;" >*</span></label>
            <input type="password" id="txtPIN2"/>
        </div>
                
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleNameDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>