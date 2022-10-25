import "./App.css";
import Form from "react-bootstrap/Form";
import "bootstrap/dist/css/bootstrap.min.css";
import {
  Card,
  Container,
  FormGroup,
  ListGroup,
  Navbar,
  Button,
  Stack,
} from "react-bootstrap";
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGithub } from "@fortawesome/free-brands-svg-icons";
import {
  faArrowUpFromBracket,
  faCheckDouble,
  faBomb,
} from "@fortawesome/free-solid-svg-icons";

class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      username: "",
      is_bedrock_player: false,
      motivation: "",
      contact: "",
      referrer: "",
      password: "",
      authenticated: false,
      requests: [],
      remove: "",
      whitelist: [],
      validated: false,
    };
  }

  render() {
    return (
      <div>
        <Navbar bg="dark" variant="dark" color="white" expand="lg" sticky="top">
          <Container className="p-1">
            <Navbar.Brand>{process.env.REACT_APP_NAME}</Navbar.Brand>
            <Navbar.Text>
              <Stack direction="horizontal" gap={3}>
                <Form
                  className="d-flex"
                  onSubmit={this.FetchRequests.bind(this)}
                >
                  <Form.Control
                    type="password"
                    id="password"
                    placeholder="Admin Login (RCON)"
                    onChange={this.handleInput.bind(this)}
                    disabled={this.state.authenticated}
                  />
                </Form>{" "}
                <a href="https://github.com/Naamloos/MangoTango" target="_">
                  <FontAwesomeIcon
                    style={{ fontSize: "2em", color: "white", margin: 0 }}
                    icon={faGithub}
                  />
                </a>
                <div></div>
              </Stack>
            </Navbar.Text>
          </Container>
        </Navbar>

        <Container className="p-3">
          <h3>Request whitelist access for {process.env.REACT_APP_NAME}</h3>
          <p>{process.env.REACT_APP_DESCRIPTION}</p>
          <p>
            Server IP: <code>{process.env.REACT_APP_MINECRAFT_IP}</code>
          </p>

          <Form
            onSubmit={this.SendRequest.bind(this)}
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
                  value={this.state.username}
                  onChange={this.handleInput.bind(this)}
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
                  onChange={this.handleRadio.bind(this)}
                  value={false}
                />

                <Form.Check
                  required
                  name="bedrock"
                  type="radio"
                  label="Bedrock Edition"
                  id="is_bedrock_player"
                  onChange={this.handleRadio.bind(this)}
                  value={true}
                />
              </div>

              <Form.Floating className="mb-3">
                <Form.Control
                  required
                  id="motivation"
                  type="text"
                  placeholder=""
                  onChange={this.handleInput.bind(this)}
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
                  onChange={this.handleInput.bind(this)}
                />
                <Form.Control.Feedback type="invalid">
                  It is important that we can notify you when you receive
                  access!
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
                  onChange={this.handleInput.bind(this)}
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

        {this.state.authenticated ? (
          <>
            <Container className="p-3">
              <h3>Current Whitelist</h3>
              <p>
                To remove members, use the{" "}
                <code>/whitelist remove [PLAYER]</code> command.
              </p>
              <ListGroup>
                {Array.from(this.state.whitelist).map((player) => (
                  <ListGroup.Item>
                    <b>{player.name}</b> ({player.uuid})
                  </ListGroup.Item>
                ))}
              </ListGroup>
            </Container>
            <Container className="p-3">
              <h3>Current requests</h3>
              {this.state.requests.length < 1 ? <p>None right now.</p> : <></>}
              {Array.from(this.state.requests).map((request) => (
                <>
                  <Card className="mb-3" style={{ width: "100%" }}>
                    <Card.Body>
                      <Card.Title>{request.username}</Card.Title>
                      <Card.Text>
                        <img
                          src={"https://crafatar.com/avatars/" + request.uuid}
                          className="rounded mx-auto d-block"
                          alt="skin"
                        />
                      </Card.Text>
                    </Card.Body>
                    <ListGroup className="list-group-flush">
                      <ListGroup.Item>
                        <b>Referrer member:</b> {request.referrer}
                      </ListGroup.Item>
                      <ListGroup.Item>
                        <b>Contact info:</b> {request.contact}
                      </ListGroup.Item>
                      <ListGroup.Item>
                        <b>Edition:</b>{" "}
                        {request.is_bedrock_player ? "Bedrock" : "Java"}
                      </ListGroup.Item>
                      <ListGroup.Item>
                        <b>UUID:</b> {request.uuid}
                      </ListGroup.Item>
                      <ListGroup.Item>
                        <b>Motivation:</b> {request.motivation}
                      </ListGroup.Item>
                    </ListGroup>
                    <Card.Body>
                      {/* Ignore the stringify I am just lost at this point */}
                      <Button
                        variant="success"
                        data={JSON.stringify(request)}
                        onClick={this.Approve.bind(this)}
                      >
                        <FontAwesomeIcon icon={faCheckDouble} /> Allow Access
                      </Button>{" "}
                      <Button
                        variant="danger"
                        data={JSON.stringify(request)}
                        onClick={this.Deny.bind(this)}
                      >
                        <FontAwesomeIcon icon={faBomb} /> Deny Access
                      </Button>{" "}
                    </Card.Body>
                  </Card>
                </>
              ))}
            </Container>
          </>
        ) : (
          <></>
        )}
      </div>
    );
  }

  handleInput(e) {
    const name = e.target.id;
    this.setState({ [name]: e.target.value });
  }

  handleRadio(e) {
    const name = e.target.id;
    const enabled = e.target.value;
    this.setState({ [name]: enabled });
  }

  validateForm(form) {
    var valid = form.checkValidity();
    this.setState({ validated: !valid });
    console.log(valid);
    return valid;
  }

  async SendRequest(e) {
    e.preventDefault();

    if (!this.validateForm(e.target)) {
      return;
    }

    const request = process.env.REACT_APP_API_ENDPOINT + "/whitelist/request";
    var req = {
      username: this.state.username,
      referrer: this.state.referrer,
      motivation: this.state.motivation,
      contact: this.state.contact,
      is_bedrock_player: this.state.is_bedrock_player === "true",
    };

    try {
      var resp = await fetch(request, {
        method: "POST",
        body: JSON.stringify(req),
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
      });

      if (resp.status !== 200) {
        throw new Error(await resp.text());
      }

      var jsonresp = await resp.json();
      alert("Succesfully sent your whitelist request!\nUUID: " + jsonresp.uuid);
    } catch (error) {
      alert(error.message);
    }
    return false;
  }

  async FetchRequests(e) {
    e.preventDefault();

    try {
      const requests =
        process.env.REACT_APP_API_ENDPOINT + "/whitelist/requests";
      var resp = await fetch(requests, {
        headers: { "X-RCON-PASSWORD": this.state.password },
      });

      if (resp.status !== 200) {
        throw new Error(await resp.text());
      }

      var list = await resp.json();
      this.setState({ authenticated: true, requests: list });
      await this.fetchCurrentWhitelist();
    } catch (error) {
      alert(error.message);
    }
  }

  async Approve(e) {
    var request = JSON.parse(e.target.attributes.getNamedItem("data").value);
    var uuid = request.uuid;
    try {
      const approve =
        process.env.REACT_APP_API_ENDPOINT + "/whitelist/approve?uuid=" + uuid;
      var resp = await fetch(approve, {
        headers: { "X-RCON-PASSWORD": this.state.password },
        method: "POST",
      });
      if (resp.status !== 200) {
        throw new Error(await resp.text());
      }
      alert("Approved whitelist access for " + request.username);
      await this.FetchRequests(e);
    } catch (error) {
      alert(error.message);
    }
  }

  async Deny(e) {
    var request = JSON.parse(e.target.attributes.getNamedItem("data").value);
    var uuid = request.uuid;
    try {
      const approve =
        process.env.REACT_APP_API_ENDPOINT + "/whitelist/deny?uuid=" + uuid;
      var resp = await fetch(approve, {
        headers: { "X-RCON-PASSWORD": this.state.password },
        method: "POST",
      });
      if (resp.status !== 200) {
        throw new Error(await resp.text());
      }
      alert("Denied whitelist access for " + request.username);
      await this.FetchRequests(e);
    } catch (error) {
      alert(error.message);
    }
  }

  async fetchCurrentWhitelist() {
    try {
      const whitelist = process.env.REACT_APP_API_ENDPOINT + "/whitelist";
      var resp = await fetch(whitelist, {
        headers: { "X-RCON-PASSWORD": this.state.password },
        method: "GET",
      });
      if (resp.status !== 200) {
        throw new Error(await resp.text());
      }
      var jsonData = await resp.json();
      this.setState({ whitelist: jsonData });
    } catch (error) {
      alert(error.message);
    }
  }
}

export default App;
