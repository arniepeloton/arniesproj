<!DOCTYPE html> 
<html> 
	<head> 
	<title>@ViewData("Title")</title>
	<meta name="viewport" content="width=device-width, initial-scale=1"> 
	<link rel="stylesheet" href="/Scripts3/others/themes/default/jquery.mobile.css" />
	<script src="/Scripts/jquery.js"></script>
	<script src="/Scripts3/others/jquery.mobile.js"></script>
</head> 
<body> 

<div data-role="page">

	<div data-role="header">
		<h1>@ViewData("Title")</h1>
	</div><!-- /header -->

	<div data-role="content">	
		@RenderBody()		
	</div><!-- /content -->

</div><!-- /page -->

</body>
</html>