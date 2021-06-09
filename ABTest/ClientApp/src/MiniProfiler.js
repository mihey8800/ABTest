/**
 * GET /mini-profiler-includes returns a <script> element
 * Re-create it in the DOM and append it to the document.
 * 
 * Using ES5 for maximum compatibilty.
 */
 (function (window) {
    if ("MiniProfiler" in window) return console.warn("MiniProfiler already imported.");

    var ERROR_MESSAGE = "MiniProfiler could not be loaded: ";

    function getIncludes(callback) {
        var xhr = new XMLHttpRequest();
        xhr.onload = function () {
            if (this.status !== 200) return callback(ERROR_MESSAGE + "request error.")
            callback(null, this.responseText);
        };
        xhr.onerror = function () {
            callback(ERROR_MESSAGE + "request error.")
        }
        xhr.open("GET", "/mini-profiler-includes", true);
        xhr.send();
    }

    function loadScript(scriptHtml) {
        if (!("DOMParser" in window) || !DOMParser.prototype.parseFromString) {
            console.error(ERROR_MESSAGE + "DOMParser::parseFromString not supported (IE9?)");
            return;
        }

        try {
            var parser = new DOMParser();
            var doc = parser.parseFromString(scriptHtml, "text/html");
            var scriptParsed = doc.querySelector("script");
            if (!scriptParsed) return;

            var script = document.createElement("script");
            for (var i = 0; i < scriptParsed.attributes.length; i++) {
                var attribute = scriptParsed.attributes[i];
                script.setAttribute(attribute.name, attribute.value);
            }

            var target = document.getElementsByTagName("script")[0];
            target.parentNode.insertBefore(script, target);
        } catch (err) {
            console.error(ERROR_MESSAGE + err.toString());
        }
    }

    getIncludes(function (err, scriptHTML) {
        if (err) return console.error(err);
        if (!scriptHTML) return;
        loadScript(scriptHTML);
    });
})(window);