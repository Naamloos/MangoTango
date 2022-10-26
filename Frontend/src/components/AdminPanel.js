import React from "react";
import Requests from "./Requests.js";
import Whitelist from "./Whitelist.js";

class AdminPanel extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    if (!this.props.authenticated) return null;

    return (
      <>
        <Whitelist
          rconPassword={this.props.rconPassword}
          logOut={this.props.logOut} />
        <Requests
          showModal={this.props.showModal}
          rconPassword={this.props.rconPassword}
          logOut={this.props.logOut} />
      </>
    );
  }
}

export default AdminPanel;
