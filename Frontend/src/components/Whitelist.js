import React from "react";
import { Container, ListGroup } from "react-bootstrap";
import { Api } from "../utils/Api.js";

class Whitelist extends React.Component {
  constructor(props) {
    super(props);

    this.state = { whitelist: [] };
  }

  componentDidMount() {
    Api.getWhitelist(
      this.props.rconPassword,
      (response) => {
        this.setState({ whitelist: response });
      },
      (error) => {}
    );
  }

  render() {
    return (
      <Container className="p-3">
        <h3>Current Whitelist</h3>
        <p>
          To remove members, use the <code>/whitelist remove [PLAYER]</code>{" "}
          command.
        </p>
        <ListGroup>
          {Array.from(this.state.whitelist).map((player) => (
            <ListGroup.Item>
              <b>{player.name}</b> ({player.uuid})
            </ListGroup.Item>
          ))}
        </ListGroup>
      </Container>
    );
  }
}

export default Whitelist;
