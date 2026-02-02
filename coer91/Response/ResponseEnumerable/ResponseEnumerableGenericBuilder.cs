namespace coer91
{ 
    public abstract class ResponseEnumerableBuilder<T> : ResponseEnumerable<T>
    {
        #region OK
        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseList<T> Ok(string message = "Success")
            => Ok([message]);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseList<T> Ok(IEnumerable<string> messageList)
            => Ok([], messageList);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseList<T> Ok(IEnumerable<T> data, string message = "Success")
            => Ok(data, [message]);

        /// <summary>
        /// HTTP Code 200
        /// </summary>
        public ResponseList<T> Ok(IEnumerable<T> data, IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 200;
            MessageList = [.. messageList];

            return new ResponseList<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList,
                Data = data
            };
        }
        #endregion


        #region Created 
        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseList<T> Created(string message = "Created")
            => Created([message]);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseList<T> Created(IEnumerable<string> messageList)
            => Created([], messageList);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseList<T> Created(IEnumerable<T> data, string message = "Created")
            => Created(data, [message]);

        /// <summary>
        /// HTTP Code 201
        /// </summary>
        public ResponseList<T> Created(IEnumerable<T> data, IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 201;
            MessageList = [.. messageList];

            return new ResponseList<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList,
                Data = data
            };
        }
        #endregion


        #region NoContent
        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseList<T> NoContent(string message = "No Content")
            => NoContent([message]);

        /// <summary>
        /// HTTP Code 204
        /// </summary>
        public ResponseList<T> NoContent(IEnumerable<string> messageList)
        {
            Failure = false;
            HttpCode = 204;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> BadRequest(string message = "Bad Request")
            => BadRequest([message]);

        /// <summary>
        /// HTTP Code 400
        /// </summary>
        public ResponseList<T> BadRequest(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 400;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> Unauthorize(string message = "Unauthorize")
            => Unauthorize([message]);

        /// <summary>
        /// HTTP Code 401
        /// </summary>
        public ResponseList<T> Unauthorize(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 401;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> Forbidden(string message = "Forbidden")
            => Forbidden([message]);

        /// <summary>
        /// HTTP Code 403
        /// </summary>
        public ResponseList<T> Forbidden(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 403;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> NotFound(string message = "Not Found")
            => NotFound([message]);

        /// <summary>
        /// HTTP Code 404
        /// </summary>
        public ResponseList<T> NotFound(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 404;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> NotAcceptable(string message = "Not Acceptable")
            => NotAcceptable([message]);

        /// <summary>
        /// HTTP Code 406
        /// </summary>
        public ResponseList<T> NotAcceptable(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 406;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> Conflict(string message = "Conflict")
            => Conflict([message]);

        /// <summary>
        /// HTTP Code 409
        /// </summary>
        public ResponseList<T> Conflict(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 409;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> PayloadTooLarge(string message = "Payload Too Large")
            => PayloadTooLarge([message]);

        /// <summary>
        /// HTTP Code 413
        /// </summary>
        public ResponseList<T> PayloadTooLarge(IEnumerable<string> messageList)
        {
            Failure = true;
            HttpCode = 413;
            MessageList = [.. messageList];

            return new ResponseList<T>()
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
        public ResponseList<T> Error(string message = "Error", int httpCode = 500)
            => Error([message], httpCode);

        /// <summary>
        /// Default: HTTP Code 500
        /// </summary>
        public ResponseList<T> Error(IEnumerable<string> messageList, int httpCode = 500)
        {
            Failure = true;
            MessageList = [.. messageList];
            HttpCode = httpCode;

            return new ResponseList<T>()
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
        public ResponseList<T> Exception(Exception ex, int httpCode = 500)
        {
            Failure = true;
            HttpCode = httpCode;
            MessageList = [ex.InnerException?.Message ?? ex.Message];

            return new ResponseList<T>()
            {
                Failure = Failure,
                HttpCode = HttpCode,
                MessageList = MessageList
            };
        }
        #endregion
    }
} 