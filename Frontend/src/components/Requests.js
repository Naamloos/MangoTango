import { faArrowsRotate } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { Container, Button, Stack } from "react-bootstrap";
import { Api } from "../utils/Api.js";
import Request from "./Request.js";

class Requests extends React.Component {
  constructor(props) {
    super(props);

    this.state = { requests: [] };
  }

  componentDidMount() {
    this.refresh();
  }

  refresh() {
    Api.getRequests(
      this.props.rconPassword,
      (response) => {
        this.setState({ requests: response });
      },
      (error) => { this.props.logOut(); }
    );
  }

  approve(request) {
    Api.approveWhitelistUser(
      request.uuid,
      this.props.rconPassword,
      () => {
        this.props.showModal(
          "Approved access!",
          "Approved whitelist access for " +
          request.username +
          "! (" +
          request.uuid +
          ")"
        );
        this.refresh();
      },
      (error) => {
        this.props.showModal(
          "Oops!",
          "Something went wrong while whitelisting: " + error
        );
      }
    );
  }

  deny(request) {
    Api.denyWhitelistUser(
      request.uuid,
      this.props.rconPassword,
      () => {
        this.props.showModal(
          "Denied access!",
          "Denied whitelist access for " +
          request.username +
          "! (" +
          request.uuid +
          ")"
        );
        this.refresh();
      },
      (error) => {
        this.props.showModal(
          "Oops!",
          "Something went wrong while denying: " + error
        );
      }
    );
  }

  render() {
    return (
      <Container className="p-3">
        <Stack direction="horizontal" gap={2}>
          <h3>Current requests</h3>
          <Button variant="secondary" size="sm" onClick={this.refresh.bind(this)}>
            <FontAwesomeIcon icon={faArrowsRotate} />
          </Button>
        </Stack>
        {this.state.requests.length < 1 ? <p>None right now.</p> : <></>}
        {Array.from(this.state.requests).map((request) => (
          <Request
            request={request}
            approve={() => {
              this.approve(request);
            }}
            deny={() => {
              this.deny(request);
            }}
          />
        ))}
      </Container>
    );
  }
}

export default Requests;
