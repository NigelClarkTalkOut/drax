var OpenWindowPlugin = {
    GetOrientation: function()
    {
    	return window.innerWidth/window.innerHeight;
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);