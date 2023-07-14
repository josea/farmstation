
<?php


//include_once 'db.php'; //initializes $conn = new mysqli();


if (get('action') == "readconfig" && isset($_GET['id'])) {
  include('db.php'); //initializes $conn = new mysqli();

  $idEscaped = $conn->real_escape_string($_GET['id']);

  $command = " SELECT config FROM configs WHERE id='" . $idEscaped . "' LIMIT 1;";
  $result = $conn->query($command);
  $config = "{}";
  while ($row = $result->fetch_row()) {

    $config = $row[0];
  }

  echo $config;
} else if (isset($_POST['token']) ) {

  $data = array();
  $user = array();

  $factory = (new Factory)->withServiceAccount( __DIR__ . '/../farmr-63a6f-firebase-adminsdk-4f7gv-4a507ac397.json');
  $auth = $factory->createAuth();

  $token = $_POST['token'];

  try {
    $verifiedIdToken = $auth->verifyIdToken($token);
  } catch (InvalidToken $e) {
    echo 'The token is invalid: ' . $e->getMessage();
    die();
  } catch (\InvalidArgumentException $e) {
    echo 'The token could not be parsed: ' . $e->getMessage();
    die();
  }

  // if you're using lcobucci/jwt ^4.0
  $uid = $verifiedIdToken->claims()->get('sub');

  $firebaseUser = $auth->getUser($uid);

  $user['username'] = $firebaseUser->displayName;
  $user['id'] = $uid;

  if (isset($firebaseUser->email))
    $user['id'] = $firebaseUser->email;

  $user['avatar'] = $firebaseUser->photoUrl;

  $data['user'] = $user;

  if (get('action') == "read") {

    $data['harvesters'] = array();

    include('db.php'); //initializes $conn = new mysqli();
    $userEscaped = $conn->real_escape_string($user['id']);

    $command = " SELECT id,data FROM farms WHERE user='" . $user['id'] . "' AND data<>'' AND data<>';;' ORDER BY lastUpdated DESC;";
    $result = $conn->query($command);

    while ($row = $result->fetch_row()) {
      $harvester = array();

      $harvester['id'] = $row[0];
      $harvester['data'] = json_decode($row[1]);

      array_push($data['harvesters'], $harvester);
    }

    echo json_encode($data);
  } else if (get('action') == "link" && isset($_GET['id'])) {
    include('db.php'); //initializes $conn = new mysqli();

    $idEscaped = $conn->real_escape_string($_GET['id']);

    $userEscaped = $conn->real_escape_string($user['id']);

    $command = "INSERT INTO farms (id, data, user) VALUES ('" . $idEscaped . "', ';;', '" . $userEscaped . "') ON DUPLICATE KEY UPDATE user=IF(user='none','" . $userEscaped . "', user)";
    $result = $conn->query($command);

    if ($result) {
      echo "success";
    }
  } else if (get('action') == "unlink" && isset($_GET['id'])) {
    include('db.php'); //initializes $conn = new mysqli();

    $idEscaped = $conn->real_escape_string($_GET['id']);
    $userEscaped = $conn->real_escape_string($user['id']);

    $command = "UPDATE farms SET user='none' WHERE user='" . $userEscaped . "' AND id='" . $idEscaped . "' LIMIT 1";
    $result = $conn->query($command);

    if ($result) {
      echo "success";
    }
  }
  //only able to save config if logged in
  else if (get('action') == "saveconfig" && isset($_GET['id']) && isset($_POST['data'])) {
    include('db.php'); //initializes $conn = new mysqli();

    $idEscaped = $conn->real_escape_string($_GET['id']);
    $dataEscaped = $conn->real_escape_string($_POST['data']);
    $userEscaped = $conn->real_escape_string($user['id']);

    $command = "SELECT id from farms WHERE id='" . $idEscaped . "' AND user='" . $userEscaped . "';";
    $result = $conn->query($command);

    if ($result->num_rows > 0) {

      $command = " INSERT INTO configs (id,config) VALUES ('" . $idEscaped . "','" . $dataEscaped . "') ON DUPLICATE KEY UPDATE config='" . $dataEscaped . "';";
      $result = $conn->query($command);

      if ($result) {
        echo "success";
      }
    }
  } else {
    //closes window and returns to dashboard
    echo "<script>
    //mobile
      // Simulate an HTTP redirect:
      window.location.replace('https://" . PREFIX . "farmr2.net/index.html');     
    </script>";
  }
} 

function get($key, $default = NULL)
{
  return array_key_exists($key, $_GET) ? $_GET[$key] : $default;
}

function session($key, $default = NULL)
{
  return array_key_exists($key, $_SESSION) ? $_SESSION[$key] : $default;
}


?>
