var express = require('express');
var router = express.Router();

/* GET users listing. */
router.get('/time/:timeNow', function(req, res, next) {
    //res.send('The time is ' + new Date().toString());
    req.query
    res.send('The time is ' + req.params.timeNow);
});

router.get('/', function(req, res, next) {
    res.send('The time is ' + new Date().toString());
});

module.exports = router;
