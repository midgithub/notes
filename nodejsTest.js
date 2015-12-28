//this is nodejs test
"use strict"; 

var modul = require("./hello2");
var name = "qing";
modul.get(name);

function main () {
	console.log("main");
	init();
}
function init () {
	console.log("init");
	//console.log(module);

	//js在浏览器中有且仅有一个全局window对象。而在Node.js环境中,也有唯一的全局对象global
	// for ( var k in global) {
	// 	console.log(k+" "+global[k]);
	// }
	console.log(global.console); 

	//Node.js进程
	console.log("Node.js进程"); 
	console.log(global.process); //true
}
main()
//module.exports是个空对象
module.exports = {
	main: main,
	init: init   //字典最后一个最好不要写 逗号
}; //模块对外输出变量,可以是任意对象、函数、数组等等