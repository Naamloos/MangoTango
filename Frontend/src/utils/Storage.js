class StorageManager {
    hasStoredPassword() {
        return window.localStorage.getItem("mangotango_token") !== null;
    }

    getStoredPassword() {
        return window.localStorage.getItem("mangotango_token");
    }

    setStoredPassword(password) {
        window.localStorage.setItem("mangotango_token", password);
    }

    clearStoredPassword() {
        window.localStorage.removeItem("mangotango_token");
    }
}

export const Storage = new StorageManager();