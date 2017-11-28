var express = require('express');
var router = express.Router();

var mysql  = require('mysql');

var connection = mysql.createConnection({     
  host     : 'localhost',       
  user     : 'sen',              
  password : '102261013028',       
  port: '3306',                   
  database: 'sen', 
});

connection.connect();

/* GET users listing. */
router.get('/', function(req, res, next) {
  var params = getRequest(req.url);

  var sql = 'SELECT * FROM codes WHERE code = ' + params["code"];
  console.log(sql);
  
  connection.query(sql,function (err, result) {
    if(err){
      console.log('[SELECT ERROR] - ',err.message);
      return;
    }
    console.log(result);
    res.send('The time is ' + new Date().toString());
  });
});

function getRequest(url) { 
  var params = {};
  if (url.indexOf("?") != -1) {
      var str = url.substr(2); //? 后面的串
      var dic = str.split("&");
      for (var index = 0; index < dic.length; index++) {
          var element = dic[index];
          var key = element.split("=")[0];
          var value = element.split("=")[1];
          params[key] = value;
      }
  }

  return params;
}

module.exports = router;
