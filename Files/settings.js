let colors = document.querySelector("#colors");

async function Save() {
    try {
        let response = await fetch("/api[PATH_PREFIX]/set?colors=" + colors.value);
        if (response.status === 200)
            window.location.reload();
        else ShowError("Connection failed.");
    } catch {
        ShowError("Connection failed.");
    }
}