class ApiClient {
  #doRequest(url, method, body, rconPassword, token) {
    return new Promise(async (success, error) => {
      const options = {
        method: "GET",
        headers: { "content-type": "application/json" },
      };

      if (rconPassword !== null)
        options.headers["X-RCON-PASSWORD"] = rconPassword;
      if (body !== null) options.body = JSON.stringify(body);
      if (method !== undefined) options.method = method;
      if (token !== null) options.headers["Authorization"] = "Bearer " + token;

      try {
        const response = await fetch(url, options);
        if (response.status === 200) {
          if (
            response.headers.get("content-type")?.includes("application/json")
          )
            // oh my god js prettier what the fuck is this, 
            // I'm this 🤏 close to dropping your formatter
            success(await response.json());
          else success(response.text());
        } else {
          error(await response.text());
        }
      } catch (err) {
        console.log(err);
        error("");
      }
    });
  }

  login(rconPassword, onSuccess, onError) {
    const url = process.env.REACT_APP_API_ENDPOINT + "/auth/login";
    this.#doRequest(url, "POST", null, rconPassword, null).then(
      (response) => onSuccess(response.token),
      (error) => onError(error)
    );
  }

  refreshToken(token, onSuccess, onError) {
    const url = process.env.REACT_APP_API_ENDPOINT + "/auth/refresh";
    this.#doRequest(url, "GET", null, null, token).then(
      (response) => onSuccess(response.token),
      (error) => onError(error)
    );
  }

  sendWhitelistRequest(request, onSuccess, onError) {
    const url = process.env.REACT_APP_API_ENDPOINT + "/whitelist/request";
    this.#doRequest(url, "POST", request).then(
      (response) => onSuccess(response),
      (error) => onError(error)
    );
  }

  approveWhitelistUser(uuid, token, onSuccess, onError) {
    const url =
      process.env.REACT_APP_API_ENDPOINT + "/whitelist/approve?uuid=" + uuid;

    this.#doRequest(url, "POST", null, null, token).then(
      (response) => onSuccess(response),
      (error) => onError(error)
    );
  }

  denyWhitelistUser(uuid, token, onSuccess, onError) {
    const url =
      process.env.REACT_APP_API_ENDPOINT + "/whitelist/deny?uuid=" + uuid;

    this.#doRequest(url, "POST", null, null, token).then(
      (response) => onSuccess(response),
      (error) => onError(error)
    );
  }

  getWhitelist(token, onSuccess, onError) {
    const url = process.env.REACT_APP_API_ENDPOINT + "/whitelist";

    this.#doRequest(url, "GET", null, null, token).then(
      (response) => onSuccess(response),
      (error) => onError(error)
    );
  }

  getRequests(token, onSuccess, onError) {
    const url = process.env.REACT_APP_API_ENDPOINT + "/whitelist/requests";

    this.#doRequest(url, "GET", null, null, token).then(
      (response) => onSuccess(response),
      (error) => onError(error)
    );
  }
}

export const Api = new ApiClient();
