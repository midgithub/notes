var http = require("http");  //require 不会重复加载模块，无论调用多少次 require， 获得的模块都是同一个。

http.createServer(function (req, res) {
    res.writeHead(200, {'Content-Type': 'text/html'});
    res.write('<h1>Node.js</h1>');
    res.end('<p>Hello World</p>');
}).listen(3000);

//显式的实现方法
var server = new http.Server();
server.on('request', function(req, res) {
    res.writeHead(200, {'Content-Type': 'text/html'});
    res.write('<h1>Node.js</h1>');
    res.end('<p>Hello World</p>');
});
server.listen(3000)

console.log("HTTP server is listening at port 3000."); //打印后没有退出，而是一直等待，直到按下 Ctrl +C 才会结束。这是因为 listen 函数中创建了事件监听器，使得 Node.js 进程不会退出事件循环。

//HTTP 服务器:  浏览器  HTTP 服务器(Apache、 IIS 或 Nginx)  PHP 解释器
//             浏览器 + node

//调试技巧：
//  PHP 总是重新读取并解析脚本,在修改 PHP 脚本后直接刷新浏览器以观察结果
//  Node.js 只有在第一次引用到某部份时才会去解析脚本文件，以后都会直接访问内存，避免重复载入,所以无论你修改了代码的哪一部份，都必须终止Node.js 再重新运行才会奏效
//  supervisor 会监视你对代码的改动，并自动重启 Node.js     npm install -g supervisors