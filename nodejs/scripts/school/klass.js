var _student = require("./student");
var _teacher = require("./teacher");

function add (teacherName,students) {
	_teacher.add(teacherName)
	students.forEach(function (item,index) {
		_student.add(item);
	});
}

module.exports = {
	add: add
};