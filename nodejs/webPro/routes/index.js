var express = require('express'); //加载express框架
var router = express.Router();

/* GET home page. */
//路由文件，相当于控制器，用于组织展示的内容
//调用模板 views/index
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Express' });
});

module.exports = router;
