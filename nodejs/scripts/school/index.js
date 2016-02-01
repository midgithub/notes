var _klass = require("./klass");

_klass.add("Sen",["stu1","stu2"]);

exports.add = function  (klasses) {
	klasses.forEach(function (item,index) {
		var klass = item;
		var teacherName = item.teacherName;
		var students = item.students;

		_klass.add(teacherName,students);
	});
}