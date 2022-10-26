import "./App.css";
import "bootstrap/dist/css/bootstrap.min.css";
import React from "react";

import RequestForm from "./components/RequestForm.js";
import SiteNavbar from "./components/SiteNavbar.js";
import AdminPanel from "./components/AdminPanel.js";
import ModalPopup from "./components/ModalPopup.js";

class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showModal: this.showModal.bind(this),
      authenticated: false,
      rconPassword: "",
    };
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
        />

        <RequestForm showModal={this.showModal.bind(this)} />

        <AdminPanel
          authenticated={this.state.authenticated}
          rconPassword={this.state.rconPassword}
          showModal={this.showModal.bind(this)}
        />

        <ModalPopup register={this.registerModal.bind(this)} />
      </div>
    );
  }

  authenticate(rconPassword) {
    this.setState({ authenticated: true, rconPassword: rconPassword });
  }

  showModal(title, message) {
    this.state.showModal(title, message);
  }

  showModalFallback(title, message) {
    alert(title + ": " + message);
  }
}

export default App;
