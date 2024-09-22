mergeInto(LibraryManager.library, {

	OnKeyboardInput: function ()
	{
		console.log("OnKeyboardInput");
		unityInstance.SendMessage('KeyboardController', 'UpdateText', document.getElementById("dummyInput").value.toString());
	}, 
	CreateDummyInput: function ()
	{
		var divElement = document.getElementById("main-container");
        var inputElement = document.createElement("input");
        inputElement.type = "text";
        inputElement.id = "dummyInput";
        inputElement.style = "font-size: 16px; position:absolute; bottom:25%;";
        divElement.appendChild(inputElement);
		inputElement.oninput = function() {console.log("ON INPUT"); window.unityInstance.SendMessage('KeyboardController', 'UpdateText', document.getElementById("dummyInput").value.toString());}
	},
	CloseInputKeyboard: function ()
	{
		document.getElementById("dummyInput").blur();
	},
	UpdateInputFieldText: function (txt)
	{
		document.getElementById("dummyInput").value = UTF8ToString(txt);
	},
	SetPointerDownOnButton: function (isDown)
	{
		window.isPointerDownOnButton = isDown;
	},
	StoreHandednessData: function (name)
	{
		if (typeof(Storage) !== "undefined") {
		  localStorage.setItem("handedness", UTF8ToString(name));
		}
	},
	GetHandednessData: function ()
	{
		if (typeof(Storage) !== "undefined") {
		  var returnStr = localStorage.getItem("handedness");
		  if(returnStr != null){
		    var bufferSize = lengthBytesUTF8(returnStr) + 1;
		    var buffer = _malloc(bufferSize);
		    stringToUTF8(returnStr, buffer, bufferSize);
			return buffer;
		  }
		}
	},
	StoreNameData: function (name)
	{
		if (typeof(Storage) !== "undefined") {
		  localStorage.setItem("playerName", UTF8ToString(name));
		}
	},
	GetNameData: function ()
	{
		if (typeof(Storage) !== "undefined") {
		  var returnStr = localStorage.getItem("playerName");
		  if(returnStr != null){
		    var bufferSize = lengthBytesUTF8(returnStr) + 1;
		    var buffer = _malloc(bufferSize);
		    stringToUTF8(returnStr, buffer, bufferSize);
			return buffer;
		  }
		}
	},
	ReloadPage: function ()
	{
		//Reload to correct domain, removing party code parameter
		if(window.location.href.includes("partycrashers.app/Testing")
		{
			window.location = "https://www.partycrashers.app/Testing";
		}
		else if(window.location.href.includes("partycrashers.app/TestingToo")
		{
			window.location = "https://www.partycrashers.app/TestingToo";
		}
		else
		{
			window.location = "https://www.partycrashers.app"
		}
	},
	CheckURLPartyCode: function ()
	{
		if(location.search.includes('?partyCode=')){
			var code = location.search.split('?partyCode=')[1];
			var bufferSize = lengthBytesUTF8(code) + 1;
		    var buffer = _malloc(bufferSize);
		    stringToUTF8(code, buffer, bufferSize);
			return buffer;
        }
		else{
			return null;
		}
	},
	TriggerHaptic: function (hapticTime)
	{
	    //Works on every browser and platform except IOS :)
		if(window.navigator.vibrate){
			navigator.vibrate(hapticTime);
		}
	}
});