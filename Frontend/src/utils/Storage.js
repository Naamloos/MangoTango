class StorageManager {
    hasStoredPassword() {
        return window.localStorage.getItem("rconPassword") !== null;
    }

    getStoredPassword() {
        return window.localStorage.getItem("rconPassword");
    }

    setStoredPassword(password) {
        window.localStorage.setItem("rconPassword", password);
    }

    clearStoredPassword() {
        window.localStorage.removeItem("rconPassword");
    }
}

export const Storage = new StorageManager();