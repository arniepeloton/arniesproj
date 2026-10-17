 <style>

    /* for table view*/
    .img1
    {
        z-index: 2;
        position: absolute;
        cursor: move;


    }
    .img2
    {
        z-index: 100;


        cursor: pointer;

        /* Set up proportionate scaling */

        height: auto;

        /* Set up positioning */
        position: fixed;
        top: 0;
        left: 0;
    }
    .gridBackground
    {
        /* Set rules to fill background */
        min-height: 100%;
	
        /* Set up proportionate scaling */
        width: 100%;
        height: auto;
	
        /* Set up positioning */
        position: fixed;
        top: 0;
        left: 0;
    }
   .cnv
    {
        z-index: 1;
        background-image: url('/Content/images/canvasGrid.png');
        background-repeat: repeat;
        top:0;
        left:0;
        width:1280px;
        height:720px;
        position:relative;


    }
    .cnvRender
    {

        background-image: url('/Content/images/renderGrid.png');
        width:100%; /* this is a must */
        height:720px; /* this is a must */
        margin:0;
        padding:0;

    }
</style>

<script>
    require([GetPathFormat('/Scripts3/TableView.js?crc=')]);
</script>


<div data-role="page" id="pageTableViewRender">
    <div id="divRenderGrid">

    </div>
</div>

