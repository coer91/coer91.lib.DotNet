namespace coer91.NET
{
    public abstract class ResponseDTOBuilder : ResponseAbstract
    {
        #region Ok
        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseDTO Ok(string message = "Success")
            => Ok([message]);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseDTO Ok(IEnumerable<string> messageList) 
        {
            Failure = false;
            HttpCode = 200; 
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO Created(string message = "Created")
            => Created([message]);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseDTO Created(IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 201;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO NoContent(string message = "No Content")
            => NoContent([message]);

        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseDTO NoContent(IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 204;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO BadRequest(string message = "Bad Request")
            => BadRequest([message]);

        /// <summary>
        /// HTTP Code 400
        /// </summary>
        public ResponseDTO BadRequest(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 400;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO Unauthorize(string message = "Unauthorize")
            => Unauthorize([message]);

        /// <summary>
        /// HTTP Code 401
        /// </summary>
        public ResponseDTO Unauthorize(IEnumerable<string> messageList) 
        {
            Failure = true;
            HttpCode = 401;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO Forbidden(string message = "Forbidden")
            => Forbidden([message]);

        /// <summary>
        /// HTTP Code 403
        /// </summary>
        public ResponseDTO Forbidden(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 403;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO NotFound(string message = "Not Found")
            => NotFound([message]);

        /// <summary>
        /// HTTP Code 404
        /// </summary>
        public ResponseDTO NotFound(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 404;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO NotAcceptable(string message = "Not Acceptable")
            => NotAcceptable([message]);

        /// <summary>
        /// HTTP Code 406
        /// </summary>
        public ResponseDTO NotAcceptable(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 406;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO Conflict(string message = "Conflict")
            => Conflict([message]);

        /// <summary>
        /// HTTP Code 409
        /// </summary>
        public ResponseDTO Conflict(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 409;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO PayloadTooLarge(string message = "Payload Too Large")
            => PayloadTooLarge([message]);

        /// <summary>
        /// HTTP Code 413
        /// </summary>
        public ResponseDTO PayloadTooLarge(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 413;
            MessageList = [.. messageList];

            return new ResponseDTO()
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
        public ResponseDTO Error(string message = "Error", int httpCode = 500)
            => Error([message], httpCode);

        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseDTO Error(IEnumerable<string> messageList, int httpCode = 500)
        {
            Failure = true;
            MessageList = [.. messageList];
            HttpCode = httpCode;

            return new ResponseDTO()
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
        public ResponseDTO Exception(Exception ex, int httpCode = 500)
        {
            Failure = true;
            HttpCode = httpCode;
            MessageList = [ex.InnerException?.Message ?? ex.Message];

            return new ResponseDTO()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        } 
        #endregion
    }
} 