async function searchWord() {
    word = (document.getElementById("search").value).toLowerCase();
    const response = await fetch("http://localhost:5282/search?word=" + word)
    const data = await response.json();

    const header = document.getElementById("resultsHeader");
    const resNotFound = document.getElementById("resNotFound");
    const spacer = document.getElementById("spacer");
    const div = document.getElementById("results");
    div.innerHTML = "";

    if (data == "Keyword not indexed") {
        header.style.display = "none";
        resNotFound.style.display = "flex";
        spacer.style.display = "flex";
    } else {
        header.style.display = "flex";
        resNotFound.style.display = "none";
        spacer.style.display = "none";

        data.forEach(item => {
            const a = document.createElement("a");
            const sec1 = document.createElement("section");
            const sec2 = document.createElement("section");

            sec1.textContent = item.articleTitle;
            sec2.textContent = getShortUrl(item.url);
            a.appendChild(sec1);
            a.appendChild(sec2);

            a.className = "result-holder";
            a.target = "_blank";
            a.href = item.url;

            div.appendChild(a);
        });
    }
}

function getShortUrl(url) {
    toReplace = "https://en.wikipedia.org/wiki/";
    return url.replace(toReplace, '');
}

const el = document.getElementById("logo");
const text = el.textContent;

const colors = ["#5de39a", "#5de39a", "#5de39a", "#5de39a", "#e5c939", "#98625e", "#98625e", "#98625e"];

el.innerHTML = "";

text.split("").forEach((char, i) => {
    const span = document.createElement("span");
    span.textContent = char;
    span.style.color = colors[i % colors.length];
    el.appendChild(span);
});

window.addEventListener("DOMContentLoaded", () => {

    document.getElementById("searchForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        console.log("submit triggered");
        await searchWord();
    });

});