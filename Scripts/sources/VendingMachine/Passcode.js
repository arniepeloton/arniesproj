require(['common'], function () {
    VendingMachine.Passcode = function (access, callback) {
        const MAX_DIGITS = 5;
        const CORRECT_PIN = "123455";
        let currentInput = "";

        const keypadGrid = document.getElementById('keypadGrid');
        const dots = document.getElementById('dotsContainer').children;
        const statusText = document.getElementById('statusText');
        const btnCancelPasscode = document.getElementById('btnCancelPasscode');

        $('#btnCancelPasscode').unbind().click(function () {
            if (access == 'MACHINE_SETTINGS') {
                $('#page_VMScreen').pageshow();
            } else {
                $('#page_inappsettings').pageshow();
            }            
        })

        // Shuffler
        function shuffle(array) {
            for (let i = array.length - 1; i > 0; i--) {
                const j = Math.floor(Math.random() * (i + 1));
                [array[i], array[j]] = [array[j], array[i]];
            }
            return array;
        }

        function generateKeypad() {
            keypadGrid.innerHTML = '';

            let numbers = Array.from({ length: 10 }, (_, i) => i);
            numbers = shuffle(numbers);

            // Row 1 to 3: Render first 9 scrambled numbers
            for (let i = 0; i < 9; i++) {
                appendNumButton(numbers[i]);
            }

            // Row 4 - Slot 1: Backspace Utility
            appendActionButton('<i class="bi-backspace"></i>', handleBackspace);

            // Row 4 - Slot 2: Last remaining scrambled number (0-9)
            appendNumButton(numbers[9]);

            // Row 4 - Slot 3: User/Gear Submit Utility
            //appendActionButton('<i class="bi-check-lg"></i>', handleSubmit, true);
            $('#page_passcode').pageshow();
        }

        // Helper: Inject number col buttons
        function appendNumButton(num) {
            const col = document.createElement('div');
            col.className = 'col d-grid';

            // Randomly apply standard or decoy active style border on load
            const decoyStyle = Math.random() > 0.5 ? 'decoy-style' : '';

            col.innerHTML = `
                <button class="btn btn-block key-btn ${decoyStyle} d-flex align-items-center justify-content-center">
                    <span class="user-select-none">${num}</span>
                </button>
            `;
            col.querySelector('button').addEventListener('click', () => handlePress(num));
            keypadGrid.appendChild(col);
        }

        // Helper: Inject control buttons
        function appendActionButton(htmlContent, callback, isSubmit = false) {
            const col = document.createElement('div');
            col.className = 'col d-grid';

            // Allow the final submit button to optionally participate as a decoy base style
            const decoyStyle = ''; // (isSubmit && Math.random() > 0.5) ? 'decoy-style' : '';

            col.innerHTML = `
                <button class="btn fs-2 btn-block key-btn ${decoyStyle} d-flex align-items-center justify-content-center fs-4 ${isSubmit && !decoyStyle ? 'border-primary-subtle textvm-primary' : ''}">
                    ${htmlContent}
                </button>
            `;
            col.querySelector('button').addEventListener('click', callback);
            keypadGrid.appendChild(col);
        }

        function handlePress(num) {
            if (currentInput.length < MAX_DIGITS) {
                currentInput += num;
                updateUI();
            }

            if (currentInput.length == MAX_DIGITS){
                handleSubmit()
            }
        }

        function handleBackspace() {
            if (currentInput.length > 0) {
                currentInput = currentInput.slice(0, -1);
                updateUI();
            }
        }

        function handleSubmit() {
            if (VendingMachine.CheckPasscode(access, currentInput) == 'allowed') {
                statusText.className = "small text-success mb-4 font-weight-bold";
                statusText.textContent = "Access Granted!";
                setTimeout(function () {
                    resetInput();
                    callback()
                }, 1500);
            } else {
                statusText.className = "small text-danger mb-4 font-weight-bold";
                statusText.textContent = "Invalid Passcode. Scrambling...";
                setTimeout(() => {
                    resetInput();
                    generateKeypad();
                }, 1200);
            }
        }

        function updateUI() {
            statusText.className = "small text-muted mb-4";
            statusText.textContent = `${currentInput.length}/${MAX_DIGITS} Digits`;

            for (let i = 0; i < MAX_DIGITS; i++) {
                if (i < currentInput.length) {
                    dots[i].classList.add('filled');
                } else {
                    dots[i].classList.remove('filled');
                }
            }
        }

        function resetInput() {
            currentInput = "";
            updateUI();
        }

        //if (access == '') {
            callback();
        //} else {
        //    generateKeypad();
        //}
    }    
})