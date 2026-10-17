require(['common'], function () {
    VendingMachine.Timer = function (container, initTime, callback) {
        const INITIAL_TIME = initTime || 30; // 30 seconds
        let timeLeft = INITIAL_TIME;
        let timerInterval = null;

        const parent = document.querySelector(container);
        const timeText = parent.querySelector('.time-display');
        const progressCircle = parent.querySelector('.timer-progress');
        const extendBtn = parent.querySelector('.extendBtn');
        const doneBtn = parent.querySelector('.doneBtn');
        const stopBtn = parent.querySelector('.stopBtn');

        // Total stroke-dasharray calculated via 2 * PI * r (2 * 3.14159 * 85)
        const circumference = 534;

        function startTimer() {
            // Clear any existing active intervals
            clearInterval(timerInterval);

            timerInterval = setInterval(() => {
                timeLeft--;
                updateUI();

                if (timeLeft <= 0) {
                    clearInterval(timerInterval);
                    handleTimeout();
                }
            }, 1000);
        }

        function updateUI() {
            // Update Text Format (00:XX)
            const secondsStr = timeLeft < 10 ? `0${timeLeft}` : timeLeft;
            timeText.textContent = `00:${secondsStr}`;

            // Calculate SVG Circle Stroke Offset
            const offset = circumference - (timeLeft / INITIAL_TIME) * circumference;
            progressCircle.style.strokeDashoffset = offset;

            // Change colors smoothly as timer gets low (under 10 seconds)
            if (timeLeft < 20 && timeLeft > 10) {
                progressCircle.style.stroke = '#ff9800'; // Turns orange
            } else if (timeLeft <= 10) {
                progressCircle.style.stroke = '#e74c3c'; // Turns red
            } else {
                progressCircle.style.stroke = '#188750';
            }
        }

        if (extendBtn != null) {
            // Logic for Extending Duration
            extendBtn.addEventListener('click', () => {
                timeLeft = INITIAL_TIME; // Reset back to 30
                updateUI();

                if (callback.Extend != undefined) {
                    clearInterval(timerInterval);
                    callback.Extend();
                } else startTimer(); // Restart ticking
            });
        }

        if (stopBtn != null) {
            // Logic for Stopping Duration
            stopBtn.addEventListener('click', () => {
                clearInterval(timerInterval);
                updateUI();
            });
        }

        if (doneBtn != null) {
            // Logic for Stopping Duration
            doneBtn.addEventListener('click', () => {
                clearInterval(timerInterval);
                handleTimeout();
            });
        }        

        function handleTimeout() {
            callback.Done();
            //alert("Time's up! The vending machine door has securely locked.");
            // In production, trigger machine door close event here.
        }

        function finishSession() {
            clearInterval(timerInterval);
            alert("Thank you for shopping with us!");
            // Redirect back to welcome/ads page.
        }

        // Initialize on Load
        updateUI();
        startTimer();
    }    
})