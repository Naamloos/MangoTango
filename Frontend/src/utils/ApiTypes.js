class WhitelistRequest {
  /**
   * @type {string}
   */
  username;

  /**
   * @type {string}
   */
  referrer;

  /**
   * @type {string}
   */
  motivation;

  /**
   * @type {string}
   */
  contact;

  /**
   * @type {boolean}
   */
  is_bedrock_player;
}

class ResolvedWhitelistRequest extends WhitelistRequest {
  /**
   * @type {string}
   */
  uuid;
}

class WhitelistUser {
  /**
   * @type {string}
   */
  uuid;

  /**
   * @type {string}
   */
  name;
}

export { WhitelistRequest, ResolvedWhitelistRequest, WhitelistUser };
