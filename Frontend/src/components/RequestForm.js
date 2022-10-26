import React from "react";
import { Form, FormGroup, Container, Button } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowUpFromBracket } from "@fortawesome/free-solid-svg-icons";
import { WhitelistRequest } from "../utils/ApiTypes.js";
import { Api } from "../utils/Api.js";

class RequestForm extends React.Component {
  constructor(props) {
    super(props);

    this.state = { validated: false, request: new WhitelistRequest() };
  }

  handleFormInput(e) {
    const name = e.target.id;

    var request = this.state.request;
    request[name] = e.target.value;

    this.setState({ request: request });
  }

  handleRadioInput(e) {
    const name = e.target.id;
    const enabled = e.target.value;

    var request = this.state.request;
    request[name] = enabled === "true";

    this.setState({ request: request });
  }

  validate(form) {
    var valid = form.checkValidity();
    this.setState({ validated: !valid });
    console.log(valid);
    return valid;
  }

  submitRequest(e) {
    e.preventDefault();
    if (!this.validate(e.target)) return;

    Api.sendWhitelistRequest(
      this.state.request,
      (response) => {
        this.props.showModal(
          "Success!",
          "Whitelist request submitted for " +
            response.username +
            "! (" +
            response.uuid +
            ")"
        );
      },
      (error) => {
        this.props.showModal("Oh no!", "Whitelist request failed!: " + error);
      }
    );
  }

  render() {
    return (
      <Container className="p-3">
        <h3>Request whitelist access for {process.env.REACT_APP_NAME}</h3>
        <p>{process.env.REACT_APP_DESCRIPTION}</p>
        <p>
          Server IP: <code>{process.env.REACT_APP_MINECRAFT_IP}</code>
        </p>

        <Form
          onSubmit={this.submitRequest.bind(this)}
          noValidate
          validated={this.state.validated}
        >
          <FormGroup>
            <Form.Floating className="mb-3">
              <Form.Control
                required
                id="username"
                type="text"
                placeholder="Notch"
                onChange={this.handleFormInput.bind(this)}
              />
              <Form.Control.Feedback type="invalid">
                A username is required!
              </Form.Control.Feedback>
              <Form.Label htmlFor="username">
                Minecraft or Xbox Username?
              </Form.Label>
            </Form.Floating>

            <div className="mb-3">
              <Form.Check
                required
                name="bedrock"
                type="radio"
                label="Java Edition"
                id="is_bedrock_player"
                onChange={this.handleRadioInput.bind(this)}
                value={false}
              />

              <Form.Check
                required
                name="bedrock"
                type="radio"
                label="Bedrock Edition"
                id="is_bedrock_player"
                onChange={this.handleRadioInput.bind(this)}
                value={true}
              />
            </div>

            <Form.Floating className="mb-3">
              <Form.Control
                required
                id="motivation"
                type="text"
                placeholder=""
                onChange={this.handleFormInput.bind(this)}
              />
              <Form.Control.Feedback type="invalid">
                Please tell us why you want to join.
              </Form.Control.Feedback>
              <Form.Label htmlFor="motivation">
                Why do you want to join?
              </Form.Label>
            </Form.Floating>

            <Form.Floating className="mb-3">
              <Form.Control
                required
                id="contact"
                type="text"
                placeholder=""
                onChange={this.handleFormInput.bind(this)}
              />
              <Form.Control.Feedback type="invalid">
                It is important that we can notify you when you receive access!
              </Form.Control.Feedback>
              <Form.Label htmlFor="motivation">
                How can we reach you?
              </Form.Label>
            </Form.Floating>

            <Form.Floating className="mb-3">
              <Form.Control
                id="referrer"
                type="text"
                placeholder=""
                onChange={this.handleFormInput.bind(this)}
              />
              <Form.Label htmlFor="referrer">
                (Optional) Who invited you?
              </Form.Label>
            </Form.Floating>
            <Button type="submit" variant="primary">
              <FontAwesomeIcon icon={faArrowUpFromBracket} /> Submit Whitelist
              Request
            </Button>
          </FormGroup>
        </Form>
      </Container>
    );
  }
}

export default RequestForm;
