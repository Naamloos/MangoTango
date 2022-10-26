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
          token={this.props.token}
          logOut={this.props.logOut} />
        <Requests
          showModal={this.props.showModal}
          token={this.props.token}
          logOut={this.props.logOut} />
      </>
    );
  }
}

export default AdminPanel;
