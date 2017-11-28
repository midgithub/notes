Express 是一个简洁而灵活的 node.js Web应用框架:

app.js是项目入口文件，package.json是npm 包管理文件，bin文件夹里面的www.js放一些全局配置项以及命令行配置等。
public 文件夹是用来存放项目静态文件目录如js,css以及图片，
routes文件夹是用来存放路由监听的代码相关文件。
views文件夹用来存放模板文件

npm install
npm start  启动服务器

2、目录结构

bin——存放命令行程序。
node_modules——存放所有的项目依赖库。
public——存放静态文件，包括css、js、img等。
routes——存放路由文件。
views——存放页面文件（ejs模板）。
app.js——程序启动文件。
package.json——项目依赖配置及开发者信息。

routes 文件夹存放路由js文件，相当于控制器，用于组织展示的内容 http://localhost:3000/users
router.get('/', function(req, res, next) {
  
   res.render('index', { title: 'Express' }); //调用模板 views/index
});

public 文件夹存资源文件 ，如 http://localhost:3000/images/rank.png