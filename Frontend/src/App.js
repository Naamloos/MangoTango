import "./App.css";
import "bootstrap/dist/css/bootstrap.min.css";
import React from "react";

import RequestForm from "./components/RequestForm.js";
import SiteNavbar from "./components/SiteNavbar.js";
import AdminPanel from "./components/AdminPanel.js";
import ModalPopup from "./components/ModalPopup.js";
import { Storage } from "./utils/Storage.js";
import { Api } from "./utils/Api.js";

class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showModal: this.showModal.bind(this),
      authenticated: false,
      token: "",
    };

    const token = Storage.getStoredPassword();
    Api.refreshToken(token,
      (newToken) => {
        Storage.setStoredPassword(newToken);
        this.setState({
          authenticated: true,
          token: newToken,
        });
      },
      () => {
        Storage.clearStoredPassword(); // error
        this.setState({
          authenticated: false,
          token: "",
        })
      }
    );
  }

  registerModal(showModal) {
    this.setState({ showModal: showModal });
  }

  render() {
    return (
      <div>
        <SiteNavbar
          showModal={this.showModal.bind(this)}
          authenticated={this.state.authenticated}
          authenticate={this.authenticate.bind(this)}
          logOut={this.logOut.bind(this)}
        />

        <RequestForm showModal={this.showModal.bind(this)} />

        <AdminPanel
          authenticated={this.state.authenticated}
          token={this.state.token}
          showModal={this.showModal.bind(this)}
          logOut={this.logOut.bind(this)}
        />

        <ModalPopup register={this.registerModal.bind(this)} />
      </div>
    );
  }

  authenticate(token) {
    Storage.setStoredPassword(token);
    this.setState({ authenticated: true, token: token });
  }

  logOut() {
    Storage.clearStoredPassword();
    this.setState({ authenticated: false, token: null });
  }

  showModal(title, message) {
    this.state.showModal(title, message);
  }

  showModalFallback(title, message) {
    alert(title + ": " + message);
  }
}

export default App;
