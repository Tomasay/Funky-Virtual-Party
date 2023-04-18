mergeInto(LibraryManager.library, {

  ShowHTMLButtonPlugin: function () {
    console.log("call from plugin");
    ShowHTMLButton();
  },
  HTMLButtonPlugin: function () {
    console.log("call from plugin");
    HTMLButton();
  },
});