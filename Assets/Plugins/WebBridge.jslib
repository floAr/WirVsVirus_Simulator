mergeInto(LibraryManager.library, {

  PushJsonData: function (str_json){
	var content = JSON.parse(Pointer_stringify(str));
	updateChart(content);
  }
});