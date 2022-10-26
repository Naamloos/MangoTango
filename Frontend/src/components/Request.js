import { faBomb, faCheckDouble } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { Card, ListGroup, Button } from "react-bootstrap";

class Request extends React.Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    return (
      <Card className="mb-3" style={{ width: "100%" }}>
        <Card.Body>
          <Card.Title>{this.props.request.username}</Card.Title>
          <Card.Text>
            <img
              src={"https://crafatar.com/avatars/" + this.props.request.uuid}
              className="rounded mx-auto d-block"
              alt="skin"
            />
          </Card.Text>
        </Card.Body>
        <ListGroup className="list-group-flush">
          <ListGroup.Item>
            <b>Referrer member:</b> {this.props.request.referrer}
          </ListGroup.Item>
          <ListGroup.Item>
            <b>Contact info:</b> {this.props.request.contact}
          </ListGroup.Item>
          <ListGroup.Item>
            <b>Edition:</b>{" "}
            {this.props.request.is_bedrock_player ? "Bedrock" : "Java"}
          </ListGroup.Item>
          <ListGroup.Item>
            <b>UUID:</b> {this.props.request.uuid}
          </ListGroup.Item>
          <ListGroup.Item>
            <b>Motivation:</b> {this.props.request.motivation}
          </ListGroup.Item>
        </ListGroup>
        <Card.Body>
          {/* Ignore the stringify I am just lost at this point */}
          <Button variant="success" onClick={this.props.approve}>
            <FontAwesomeIcon icon={faCheckDouble} /> Allow Access
          </Button>{" "}
          <Button variant="danger" onClick={this.props.deny}>
            <FontAwesomeIcon icon={faBomb} /> Deny Access
          </Button>{" "}
        </Card.Body>
      </Card>
    );
  }
}

export default Request;
