using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class HtmlSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Oranges: strings with double quotes
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Orange}>{m.Value}</color>");

            // Grays: Comments
            code = Regex.Replace(code, @"<!--.*?-->", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Greens: tag names (e.g., <html>, <body>, <div>, <span>, <a>, <img>, <input>, <button>, <form>, <table>, <tr>, <td>, <th>, <ul>, <ol>, <li>, <h1>, <h2>, <h3>, <h4>, <h5>, <h6>, <p>, <br>, <hr>, <strong>, <em>, <i>, <b>, <u>, <s>, <sub>, <sup>, <small>, <big>, <pre>, <code>, <blockquote>, <q>, <cite>, <abbr>, <acronym>, <address>, <time>, <date>, <time>, <dfn>, <kbd>, <samp>, <var>, <mark>, <ruby>, <rt>, <rp>, <bdi>, <bdo>, <wbr>, <details>, <summary>, <menu>, <menuitem>, <command>, <section>, <nav>, <article>, <aside>, <header>, <footer>, <main>, <figure>, <figcaption>, <div>, <span>, <a>, <img>, <input>, <button>, <form>, <table>, <tr>, <td>, <th>, <ul>, <ol>, <li>, <h1>, <h2>, <h3>, <h4>, <h5>, <h6>, <p>, <br>, <hr>, <strong>, <em>, <i>, <b>, <u>, <s>, <sub>, <sup>, <small>, <big>, <pre>, <code>, <blockquote>, <q>, <cite>, <abbr>, <acronym>, <address>, <time>, <date>, <time>, <dfn>, <kbd>, <samp>, <var>, <mark>, <ruby>, <rt>, <rp>, <bdi>, <
            code = Regex.Replace(code, @"<\w+>", m => $"<color={Colors.Green}>{m.Value}</color>");

            // Blues: attribute names (e.g., href, src, alt, title, id, class, style, width, height, colspan, rowspan, checked, selected, disabled, readonly, multiple, placeholder, required, pattern, min, max, step, value, name, type, action, method, target, rel, media, hreflang, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes, type, charset, name, content, http-equiv, scheme, async, defer, src, type, charset, language, xml:lang, dir, href, rel, media, sizes,
            code = Regex.Replace(code, @"\b(href|src|alt|title|id|class|style|width|height|colspan|rowspan|checked|selected|disabled|readonly|multiple|placeholder|required|pattern|min|max|step|value|name|type|action|method|target|rel|media|hreflang|charset|name|content|http-equiv|scheme|async|defer|src|type|charset|language|xml:lang|dir|sizes)\b", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Reds: attribute values (strings with double quotes)
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Red}>{m.Value}</color>");

            return code;
        }
    }
}