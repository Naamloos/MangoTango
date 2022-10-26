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
      authenticated: Storage.hasStoredPassword(),
      rconPassword: Storage.getStoredPassword(),
    };

    const password = Storage.getStoredPassword();
    Api.checkAuth(password,
      () => {
        this.setState({
          authenticated: true,
          rconPassword: password,
        })
      },
      () => {
        Storage.clearStoredPassword(); // error
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
          rconPassword={this.state.rconPassword}
          showModal={this.showModal.bind(this)}
          logOut={this.logOut.bind(this)}
        />

        <ModalPopup register={this.registerModal.bind(this)} />
      </div>
    );
  }

  authenticate(rconPassword) {
    Storage.setStoredPassword(rconPassword);
    this.setState({ authenticated: true, rconPassword: rconPassword });
  }

  logOut() {
    Storage.clearStoredPassword();
    this.setState({ authenticated: false, rconPassword: null });
  }

  showModal(title, message) {
    this.state.showModal(title, message);
  }

  showModalFallback(title, message) {
    alert(title + ": " + message);
  }
}

export default App;
