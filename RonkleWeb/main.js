async function searchWord() {
    console.log("searchWord called");
    word = (document.getElementById("search").value).toLowerCase();
    const response = await fetch("http://localhost:5282/search?word=" + word)
    const data = await response.json();
    console.log(data);
    
    const div = document.getElementById("results");
    div.innerHTML = "";

    data.forEach(item => {
        const a = document.createElement("a");
        a.textContent = item.articleTitle;
        a.target = "_blank";
        a.href = item.url;
        div.appendChild(a);
    });
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