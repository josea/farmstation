<?php

include('db.php');

if ($conn->connect_errno) {
    echo "Failed to connect to database!";
    error_log('Failed to connect to database!');
}
error_log('id is set: ' . (isset($_POST['id'])? 'true': 'false'));
error_log('data is set: ' . (isset($_POST['data'])? 'true': 'false'));
echo 'id is set: ' . (isset($_POST['id'])? 'true': 'false') . '<br/>';
echo 'data is set: ' . (isset($_POST['data'])? 'true': 'false') . '<br/>';

print_r($_POST);
if (isset($_POST['id']) && isset($_POST['data'])) {
    $id = $conn->real_escape_string($_POST['id']);
    
    $data = $conn->real_escape_string($_POST['data']);
    $publicAPI = $conn->real_escape_string($_POST['publicAPI']);

    $command = "SELECT id from farms WHERE id='" . $id . "' AND user<>'none';";
    $result = $conn->query($command);

    if ($result->num_rows > 0) {

        $name = $conn->real_escape_string($_POST['name']);

        $command = " UPDATE farms SET publicAPI=" . $publicAPI . ", data='" . $data . "', lastUpdated = utc_timestamp() WHERE id='" . $id . "' AND user<>'none' AND lastUpdated < DATE_SUB(utc_timestamp(), INTERVAL 1 MINUTE) LIMIT 1;";
        $result = $conn->query($command);

        // $f = fopen("log.txt", "w");
        // fwrite($f, 'Updated farms' . PHP_EOL);
       
        error_log('About to check extra variables!');
        //if one of these variables are set, then it needs to find user id 
        if (isset($_POST['balance']) || isset($_POST['coldBalance']) || isset($_POST['lastPlot'])
         || isset($_POST['notifyOffline']) || isset($_POST['isFarming'])
         || isset($_POST['drives'])) {
            $user = "none";

            error_log('Found extra variables!');

             $balanceStr = isset($_POST['balance']) ? $_POST['balance'] : -1; 
             $coldBalanceStr = isset($_POST['coldBalance']) ? $_POST['coldBalance'] : -1; 
             $lastPlotStr = isset($_POST['lastPlot']) ? $_POST['lastPlot'] : -1; 
             $notifyOfflineStr = isset($_POST['notifyOffline']) ? $_POST['notifyOffline'] : -1; 
             $isFarmingStr = isset($_POST['isFarming']) ? $_POST['isFarming'] : -1; 
             $drivesStr = isset($_POST['drives']) ? $_POST['drives'] : -1; 

            error_log('Going to update more detail ' . PHP_EOL);
            error_log('    Balance ' . $balanceStr . PHP_EOL);
            error_log('    coldBalance ' . $coldBalanceStr . PHP_EOL);
            error_log('    lastplot ' . $lastPlotStr . PHP_EOL);
            error_log('    notifyOffline ' . $notifyOfflineStr . PHP_EOL);
            error_log('    isFarming ' . $isFarmingStr . PHP_EOL);
            error_log('    drives ' . $drivesStr . PHP_EOL);
            
            //searches for user id which is linked to client id, so that the discord bot can message that person
            $getUser = " SELECT user from farms WHERE id='" . $id . "'";
            $result2 = $conn->query($getUser);

            while ($row = $result2->fetch_row()) {
                $user = $row[0];
            }

            if (isset($_POST['lastPlot']) && $_POST['lastPlot'] != "0") {

                $lastPlot = $conn->real_escape_string($_POST['lastPlot']);
                // fwrite($f, 'Select lastplot' . PHP_EOL);

                $checkIfPlots = "SELECT lastplot from lastplots WHERE id='" . $id . "';";
                $result3 = $conn->query($checkIfPlots);

                $existsPlot = false;
                $previousID = "0";

                while ($row = $result3->fetch_row()) {
                    $existsPlot = true;
                    $previousID = $row[0];
                }

                $command2 = "";

                //If there doesnt exist an entry with last plot
                if (!$existsPlot) {
                    $command2 = " INSERT INTO lastplots (id, lastplot) VALUES ('" . $id . "','" . $lastPlot . "');";
                    $conn->query($command2);
                }
                //If there is an entry with last plot and its different from previous registered plot id then update it and notify user
                else if (($previousID !== $lastPlot) && (gettype($previousID) === gettype($lastPlot))) {
                    $command2 = " UPDATE lastplots set lastplot='" . $lastPlot . "' WHERE id='" . $id . "';";
                    $conn->query($command2);

                    //send
                    $arg = "plot";
                    //send notification
                    $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                    $conn->query($commandNotif);
                }
            }

            if (isset($_POST['balance'])) {
                error_log('Updating balance');
                $balance = floatval($conn->real_escape_string($_POST['balance']));

                //checks stored balance, or if there is an entry in the database
                $checkBalance = "SELECT balance from balances WHERE id='" . $id . "';";
                $result4 = $conn->query($checkBalance);

                $existsBalance = false;
                $previousBalance = floatval('0.0');

                while ($row = $result4->fetch_row()) {
                    $existsBalance = true;
                    $previousBalance = floatval($row[0]);
                }

                $command3 = "";

                //If there doesnt exist an entry with last balance
                if (!$existsBalance) {
                    $command3 = " INSERT INTO balances (id, balance) VALUES ('" . $id . "','" . $balance . "');";
                    $conn->query($command3);
                }
                //If there is an entry with last balance and its a higher value than previous registered balance then update it and notify user
                else {
                    $command3 = " UPDATE balances set balance='" . $balance . "' WHERE id='" . $id . "';";
                    $conn->query($command3);

                    if ($balance > $previousBalance) {
                        //send
                        $arg = "block";
                        //send notification
                        $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                        $conn->query($commandNotif);
                    }
                }
            }

            if (isset($_POST['coldBalance'])) {
                error_log('Updating cold balance');
                $balance = floatval($conn->real_escape_string($_POST['coldBalance']));

                //checks stored balance, or if there is an entry in the database
                $checkBalance = "SELECT balance from balances WHERE id='cold" . $id . "';";
                $result4 = $conn->query($checkBalance);

                $existsBalance = false;
                $previousBalance = floatval('0.0');

                while ($row = $result4->fetch_row()) {
                    $existsBalance = true;
                    $previousBalance = floatval($row[0]);
                }

                $command3 = "";

                //If there doesnt exist an entry with last balance
                if (!$existsBalance) {
                    $command3 = " INSERT INTO balances (id, balance) VALUES ('cold" . $id . "','" . $balance . "');";
                    $conn->query($command3);
                }
                //If there is an entry with last balance and its a higher value than previous registered balance then update it and notify user
                else {
                    $command3 = " UPDATE balances set balance='" . $balance . "' WHERE id='cold" . $id . "';";
                    $conn->query($command3);

                    //send notification
                    if ($balance > $previousBalance) {
                        $arg = "coldBlock";
                        //send notification
                        $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                        $conn->query($commandNotif);
                    }
                }
            }

            if (isset($_POST['drives'])) {
                error_log('Updating drives');
                $drives = floatval($conn->real_escape_string($_POST['drives']));

                //checks stored drives, or if there is an entry in the database
                $checkDrives = "SELECT drives from drives WHERE id='" . $id . "';";
                $result4 = $conn->query($checkDrives);

                $existsDrives = false;
                $previousDrives = floatval('9999.0'); //defaults to a very high number

                while ($row = $result4->fetch_row()) {
                    $existsDrives = true;
                    $previousDrives = floatval($row[0]);
                }

                $command3 = "";

                //If there doesnt exist an entry with last drive count
                if (!$existsDrives) {
                    $command3 = " INSERT INTO drives (id, drives) VALUES ('" . $id . "','" . $drives . "');";
                    $conn->query($command3);
                }
                //If there is an entry with last drive count and its a lower value than previous registered drive countthen update it and notify user that a hard drive disconnected
                else if ($drives != $previousDrives) {
                    $command3 = " UPDATE drives set drives='" . $drives . "' WHERE id='" . $id . "';";
                    $conn->query($command3);

                    if ($drives < $previousDrives) {
                        //send
                        $arg = "drive";
                        //send notification
                        $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                        $conn->query($commandNotif);
                    }
                }
            }

            if (isset($_POST['notifyOffline'])) {
                error_log('Updating notifyoffline');
                $notify = $conn->real_escape_string($_POST['notifyOffline']);

                $command4 = " INSERT INTO offline (id, notify, name) VALUES ('" . $id . "','" . $notify . "', '" . $name . "') ON DUPLICATE KEY UPDATE notify='" . $notify . "', name='" . $name . "' ;";
                $conn->query($command4);
            }

            if (isset($_POST['isFarming'])) {
                error_log('Updating isFarming');
                //0 means it is not farming/harvesting
                //1 means it is farming/harvesting
                //2 means it is/was offline
                $isFarming = (int) $conn->real_escape_string($_POST['isFarming']);

                $checkIfFarming = "SELECT isfarming from statuses WHERE id='" . $id . "';";
                $result5 = $conn->query($checkIfFarming);

                $existsEntry = false;
                $previousValue = "0";

                while ($row = $result5->fetch_row()) {
                    $existsEntry = true;
                    $previousValue = (int) $row[0];
                }
                
                if ($isFarming === 1 ) {
                    $isFarmingText = "FARMING";
                }else if($isFarming ==2){
                    $isFarmingText = "OFFLINE";
                }else{
                    $isFarmingText = "STOPPED";
                }

                $command5 = "";

                 $commandJoseA1 = "UPDATE farms SET farmingstatus = '" . $isFarmingText . "', farmingstatustimestamp = utc_timestamp() WHERE id='" . $id . "' and farmingstatus <> '" . $isFarmingText . "';";
                 $conn->query($commandJoseA1);

                 if ($isFarming === 1){
                     $commandJoseA1 = "UPDATE farms SET lastfarming = utc_timestamp() WHERE id='" . $id . "' ;";
                     $conn->query($commandJoseA1);
                 }

                //If there doesnt exist an entry with last isfarming
                if (!$existsEntry) {
                    $command5 = " INSERT INTO statuses (id, isfarming, lastupdated) VALUES ('" . $id . "','" . $isFarming . "', utc_timestamp());";
                    $conn->query($command5);                   
                }
                //If there is an entry with last plot and its different from previous registered plot id then update it and notify user
                else if (($isFarming !== $previousValue) && (gettype($isFarming) === gettype($previousValue))) {
                    $command5 = " UPDATE statuses set isfarming='" . $isFarming . "', lastupdated = utc_timestamp() WHERE id='" . $id . "';";
                    $conn->query($command5);

                    // removed this as it was showing generating many errors in the email notifications
                    // probably handle this in the background service.

                    //send notification if client was previously farming but now its not
                   /* if ($previousValue === 1 && $isFarming === 0) {
                        $arg = "stopped";
                        //send notification
                        $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                        $conn->query($commandNotif);
                    }
                    //send notification if client was not farming but now it is
                    else if ($previousValue === 0 && $isFarming === 1) {
                        $arg = "started";
                        //send notification
                        $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                        $conn->query($commandNotif);
                    }
                    //send notification if client was not farming but now it is
                    else if ($previousValue === 2) {
                        $arg = "online";
                        //send notification
                        $commandNotif = " INSERT INTO notifications(user,type,name) VALUES ('" . $user . "', '" . $arg . "', '" . $name . "');";
                        $conn->query($commandNotif);
                    }*/
                }
            }
        }
        // fclose($f);
    } else {
        echo "Not linked";

        $command = " INSERT INTO farms (id, data, user, publicAPI) VALUES ('" . $id . "','" . $data . "', 'none', " . $publicAPI . ") ;";
       // $f = fopen("log.txt", "w");
       // fwrite($f, 'Not linked\n' . $command);
       // fclose($f);
        $result = $conn->query($command);
    }
}


$conn->close();
