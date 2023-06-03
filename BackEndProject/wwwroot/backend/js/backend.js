const loadMoreBtn = document.getElementById("loadMoreBtn");
const courseList = document.getElementById("courseList");
const courseCount = document.getElementById("courseCount").value;

let skip = 6;
loadMoreBtn.addEventListener("click", function () {
    fetch(`/Course/LoadMore?skip=${skip}`).then(response => response.text())
        .then(data => {
            courseList.innerHTML += data;
        })
    skip += 6;
    if (skip >= courseCount) {
        loadMoreBtn.remove();
    }
})