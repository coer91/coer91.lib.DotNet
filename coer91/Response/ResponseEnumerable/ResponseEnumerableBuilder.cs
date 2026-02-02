namespace coer91
{
    public abstract class ResponseEnumerableBuilder : Response
    {
        #region Ok
        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseList Ok(string message = "Success")
            => Ok([message]);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseList Ok(IEnumerable<string> messageList) 
        {
            Failure = false;
            HttpCode = 200; 
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Created
        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseList Created(string message = "Created")
            => Created([message]);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseList Created(IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 201;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region NoContent
        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseList NoContent(string message = "No Content")
            => NoContent([message]);

        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseList NoContent(IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 204;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region BadRequest
        /// <summary>
        /// HTTP Code 400
        /// </summary>
        public ResponseList BadRequest(string message = "Bad Request")
            => BadRequest([message]);

        /// <summary>
        /// HTTP Code 400
        /// </summary>
        public ResponseList BadRequest(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 400;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Unauthorize
        /// <summary>
        /// HTTP Code 401
        /// </summary>
        public ResponseList Unauthorize(string message = "Unauthorize")
            => Unauthorize([message]);

        /// <summary>
        /// HTTP Code 401
        /// </summary>
        public ResponseList Unauthorize(IEnumerable<string> messageList) 
        {
            Failure = true;
            HttpCode = 401;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Forbidden
        /// <summary>
        /// HTTP Code 403
        /// </summary>
        public ResponseList Forbidden(string message = "Forbidden")
            => Forbidden([message]);

        /// <summary>
        /// HTTP Code 403
        /// </summary>
        public ResponseList Forbidden(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 403;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region NotFound
        /// <summary>
        /// HTTP Code 404
        /// </summary>
        public ResponseList NotFound(string message = "Not Found")
            => NotFound([message]);

        /// <summary>
        /// HTTP Code 404
        /// </summary>
        public ResponseList NotFound(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 404;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region NotAcceptable
        /// <summary>
        /// HTTP Code 406
        /// </summary>
        public ResponseList NotAcceptable(string message = "Not Acceptable")
            => NotAcceptable([message]);

        /// <summary>
        /// HTTP Code 406
        /// </summary>
        public ResponseList NotAcceptable(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 406;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Conflict
        /// <summary>
        /// HTTP Code 409
        /// </summary>
        public ResponseList Conflict(string message = "Conflict")
            => Conflict([message]);

        /// <summary>
        /// HTTP Code 409
        /// </summary>
        public ResponseList Conflict(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 409;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region PayloadTooLarge
        /// <summary>
        /// HTTP Code 413
        /// </summary>
        public ResponseList PayloadTooLarge(string message = "Payload Too Large")
            => PayloadTooLarge([message]);

        /// <summary>
        /// HTTP Code 413
        /// </summary>
        public ResponseList PayloadTooLarge(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 413;
            MessageList = [.. messageList];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion


        #region Error
        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseList Error(string message = "Error", int httpCode = 500)
            => Error([message], httpCode);

        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseList Error(IEnumerable<string> messageList, int httpCode = 500)
        {
            Failure = true;
            MessageList = [.. messageList];
            HttpCode = httpCode;

            return new ResponseList()
            {
                Failure = Failure,
                MessageList = MessageList,
                HttpCode = HttpCode
            };
        }
        #endregion


        #region Exception
        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseList Exception(Exception ex, int httpCode = 500)
        {
            Failure = true;
            HttpCode = httpCode;
            MessageList = [ex.InnerException?.Message ?? ex.Message];

            return new ResponseList()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        } 
        #endregion
    }
} 