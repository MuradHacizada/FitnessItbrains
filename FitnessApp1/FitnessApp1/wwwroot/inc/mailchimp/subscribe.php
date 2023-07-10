<?php
	require_once 'MCAPI.class.php';
	$api = new MCAPI('5494f6b090a183d0d5f53b5102e4b361-us3');
	$merge_vars = array('FNAME'=>$_POST["fname"]);

	// Submit subscriber data to MailChimp
	// For parameters doc, refer to: http://apidocs.mailchimp.com/api/1.3/listsubscribe.func.php
	$retval = $api->listSubscribe( '45e47c5c81', $_POST["email"], $merge_vars, 'html', false, true );

	if ($api->errorCode){
		echo "<h4>Please try again.</h4>" . $api->errorCode;
	} else {
		echo "<h4 class='success'>Thank you, you have been added to our mailing list.</h4>";
	}
?>
