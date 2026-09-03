// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const questions = document.querySelectorAll(".question-card");

    const nextButton = document.getElementById("nextButton");
    const backButton = document.getElementById("backButton");

    const questionNumber = document.getElementById("questionNumber");
    const progressPercentage = document.getElementById("progressPercentage");
    const progressFill = document.getElementById("progressFill");

    if (!questions.length || !nextButton || !backButton) {
        return;
    }

    let currentQuestion = 0;

    function updateQuestion() {

        questions.forEach(function (question, index) {

            question.classList.toggle(
                "active-question",
                index === currentQuestion
            );

        });

        const currentNumber = currentQuestion + 1;
        const percentage = currentNumber * 20;

        questionNumber.textContent =
            `Question ${currentNumber} of 5`;

        progressPercentage.textContent =
            `${percentage}%`;

        progressFill.style.width =
            `${percentage}%`;

        backButton.style.visibility =
            currentQuestion === 0
                ? "hidden"
                : "visible";

        nextButton.textContent =
            currentQuestion === questions.length - 1
                ? "Create My Profile"
                : "Continue";
    }


    questions.forEach(function (question) {

        const answers =
            question.querySelectorAll(".answer-card");

        answers.forEach(function (answer) {

            answer.addEventListener("click", function () {

                answers.forEach(function (item) {
                    item.classList.remove("selected-answer");
                });

                answer.classList.add("selected-answer");

            });

        });

    });


    nextButton.addEventListener("click", function () {

        const selectedAnswer =
            questions[currentQuestion]
                .querySelector(".selected-answer");

        if (!selectedAnswer) {
            return;
        }

        if (currentQuestion < questions.length - 1) {

            currentQuestion++;
            updateQuestion();

        } else {

            alert("Your SereGlowy profile is ready!");

        }

    });


    backButton.addEventListener("click", function () {

        if (currentQuestion > 0) {

            currentQuestion--;
            updateQuestion();

        }

    });


    updateQuestion();

});
